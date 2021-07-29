using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Transactions;
using UltraPro.API.Core;
using UltraPro.API.Models;
using UltraPro.Common.Constants;
using UltraPro.Common.Enums;
using UltraPro.Common.Services;
using UltraPro.Entities;
using UltraPro.Common.Exceptions;
using UltraPro.Services.Interfaces;

namespace UltraPro.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppSettings _appSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IMailerService _mailerService;

        //TODO: Need to update IgnoreQueryFilters
        public AuthenticationService(
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            IRefreshTokenService refreshTokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<AppSettings> appSettings,
            IMailerService mailerService,
            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
            _refreshTokenService = refreshTokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _mailerService = mailerService;
        }

        public async Task<ApplicationUser> AuthenticatePortalAsync(ApiLoginModel model)
        {
            var user = await this._userManager.FindByNameAsync(model.UserName);
            if (user == null) throw new ValidationException("UserName or password is invalid.");

            var isValidUser = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isValidUser)
            {
                user.AccessFailedCount += 1;
                var result = await _userManager.UpdateAsync(user); 
                
                throw new ValidationException("UserName or password is invalid.");
            }

            return user;
        }
        
        public async Task<ApplicationUser> AuthenticateAppAsync(ApiAppLoginModel model, HttpContext context)
        {
            var header = context.Request?.Headers[nameof(_appSettings.AppHeaderSecretKey)];

            if (!header.HasValue || !header.Equals(_appSettings.AppHeaderSecretKey)) throw new ValidationException("Invalid request.");

            var user = await this._userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) throw new ValidationException("Phone Number or password is invalid.");

            var isValidUser = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isValidUser)
            {
                user.AccessFailedCount += 1;
                var result = await _userManager.UpdateAsync(user);

                throw new ValidationException("Phone Number or password is invalid.");
            }

            return user;
        }
        
        public async Task<ApplicationUser> RegistrationAppAsync(ApiAppRegistrationModel model, HttpContext context)
        {
            var headerSK = context.Request?.Headers[nameof(_appSettings.AppHeaderSecretKey)];

            if (!headerSK.HasValue || !headerSK.Equals(_appSettings.AppHeaderSecretKey)) throw new ValidationException("Invalid request.");

            var user = _mapper.Map<ApiAppRegistrationModel, ApplicationUser>(model);

            #region registration
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var existingUser = (await _userManager.FindByNameAsync(user.UserName));
                    if (existingUser != null && !existingUser.IsDeleted)
                        throw new DuplicationException(nameof(user.PhoneNumber));

                    user.Status = EnumApplicationUserStatus.GeneralUser;
                    user.Created = _dateTime.Now;
                    var userSaveResult = await _userManager.CreateAsync(user, model.Password);

                    if (!userSaveResult.Succeeded) throw new IdentityValidationException(userSaveResult.Errors);

                    // Add New User Role
                    var userRoleName = ConstantsUserRole.AppUser;
                    var role = await _roleManager.FindByNameAsync(userRoleName);

                    if (role == null) throw new NotFoundException(nameof(ApplicationRole), userRoleName);

                    var roleSaveResult = await _userManager.AddToRoleAsync(user, role.Name);

                    if (!roleSaveResult.Succeeded) throw new IdentityValidationException(roleSaveResult.Errors);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
            #endregion

            user = await this._userManager.FindByNameAsync(user.UserName);

            return user;
        }

        public async Task<bool> ChangePasswordAppAsync(ApiAppChangePasswordModel model, HttpContext context)
        {
            var header = context.Request?.Headers[nameof(_appSettings.AppHeaderSecretKey)];

            if (!header.HasValue || !header.Equals(_appSettings.AppHeaderSecretKey)) throw new ValidationException("Invalid request.");

            var user = await this._userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) throw new ValidationException("Phone Number or current password is invalid.");

            var isValidUser = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isValidUser) throw new ValidationException("Phone Number or current password is invalid.");

            user.PasswordChangedCount = user.PasswordChangedCount + 1;
            await _userManager.UpdateAsync(user);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded) throw new IdentityValidationException(result.Errors);

            return true;
        }

        public async Task<bool> ForgotPasswordPortalAsync(string email)
        {
            var user = await this._userManager.FindByEmailAsync(email);
            if (user == null) throw new ValidationException("Email is invalid.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = $"{this._appSettings.ResetPasswordUrl}?userId={user.Id}&token={token}";

            //email = "mahmud.koli@brainstation23.com";
            await _mailerService.SendEmailAsync(email, "Forgot Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return true;
        }

        public async Task<bool> ResetPasswordPortalAsync(ApiResetPasswordModel model)
        {
            var user = await this._userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null) throw new ValidationException("User Id or token is invalid.");

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded) throw new ValidationException("User Id or token is invalid.");

            return true;
        }

        public async Task<ApiAuthenticateUserModel> AuthenticateTokenAsync(ApplicationUser user, HttpContext context, HttpRequest request, HttpResponse response)
        {
            var ipAddress = IpAddress(context, request);
            var jwtToken = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken(user, ipAddress);

            await _refreshTokenService.AddAsync(refreshToken);

            var authUser = _mapper.Map<ApplicationUser, ApiAuthenticateUserModel>(user);
            authUser.JwtToken = jwtToken;
            authUser.RefreshToken = refreshToken.Token;

            SetTokenCookie(response, authUser.RefreshToken);

            return authUser;
        }

        public async Task<ApiAuthenticateUserModel> RefreshTokenAsync(HttpContext context, HttpRequest request, HttpResponse response)
        {
            var token = GetTokenCookie(request);
            var ipAddress = IpAddress(context, request);
            var refreshToken = await _refreshTokenService.GetByTokenAsync(token);
            if (refreshToken == null || !refreshToken.IsActive) throw new Exception("Invalid token.");

            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user == null) throw new Exception("Invalid token.");

            var newRefreshToken = GenerateRefreshToken(user, ipAddress);
            refreshToken.Revoked = _dateTime.Now;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            await _refreshTokenService.AddAsync(newRefreshToken);
            await _refreshTokenService.UpdateAsync(refreshToken);

            var jwtToken = await GenerateJwtTokenAsync(user);

            var authUser = _mapper.Map<ApplicationUser, ApiAuthenticateUserModel>(user);
            authUser.JwtToken = jwtToken;
            authUser.RefreshToken = newRefreshToken.Token;

            SetTokenCookie(response, authUser.RefreshToken);

            return authUser;
        }

        public async Task<bool> RevokeTokenAsync(ApiAppRevokeTokenModel model, HttpContext context, HttpRequest request)
        {
            var token = model.Token ?? GetTokenCookie(request);
            var ipAddress = IpAddress(context, request);

            if (string.IsNullOrEmpty(token))
                throw new Exception("Token is required.");

            var refreshToken = await _refreshTokenService.GetByTokenAsync(token);
            if (refreshToken == null || !refreshToken.IsActive) throw new Exception("Token not found.");

            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user == null) throw new Exception("Token not found.");

            refreshToken.Revoked = _dateTime.Now;
            refreshToken.RevokedByIp = ipAddress;
            await _refreshTokenService.UpdateAsync(refreshToken);

            return true;
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._appSettings.TokenSecretKey);

            var claims = new List<Claim>()
            {
                //new Claim(ClaimTypes.Name, user.Id.ToString())
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email??string.Empty),
                new Claim(ClaimTypes.Name, user.UserName??string.Empty)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) { continue; }
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (Claim roleClaim in roleClaims)
                {
                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = _dateTime.Now.AddHours(this._appSettings.TokenExpiresHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(ApplicationUser user, string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    UserId = user.Id,
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        private void SetTokenCookie(HttpResponse Response, string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        
        private string GetTokenCookie(HttpRequest Request)
        {
            return Request.Cookies["refreshToken"];
        }

        private string IpAddress(HttpContext HttpContext, HttpRequest Request)
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
