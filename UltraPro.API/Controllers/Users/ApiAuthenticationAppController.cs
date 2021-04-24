using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UltraPro.API.Models;
using UltraPro.Entities;
using UltraPro.API.Core;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using UltraPro.API.Controllers.Common;
using UltraPro.Services.Exceptions;
using UltraPro.Common.Constants;
using UltraPro.Services.Interfaces;
using UltraPro.Common.Enums;
using System.Transactions;
using UltraPro.Common.Services;

namespace UltraPro.API.Controllers.Users
{
    //[AllowAnonymous]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/app/auth")]
    public class ApiAuthenticationAppController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ApiAuthenticationAppController(
            IOptionsSnapshot<AppSettings> options,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IDateTime dateTime,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dateTime = dateTime;
            _mapper = mapper;
            _appSettings = options.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiAppLoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var header = HttpContext.Request?.Headers[nameof(_appSettings.AppHeaderSecretKey)];

                if (!header.HasValue || !header.Equals(_appSettings.AppHeaderSecretKey))
                {
                    ModelState.AddModelError("", "Invalid request.");
                    return ValidationResult(ModelState);
                }

                var user = await this._userManager.FindByNameAsync(model.PhoneNumber);
                if (user == null)
                {
                    ModelState.AddModelError("", "Phone Number or password is invalid.");
                    return ValidationResult(ModelState);
                }

                var isValidUser = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isValidUser)
                {
                    user.AccessFailedCount += 1;
                    var result = await _userManager.UpdateAsync(user);

                    ModelState.AddModelError("", "Phone Number or password is invalid.");
                    return ValidationResult(ModelState);
                }

                var authUser = _mapper.Map<ApplicationUser, ApiAuthenticateUserModel>(user);

                // authentication successful so generate jwt token
                authUser.Token = await this.BuildToken(user);

                return OkResult(authUser);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] ApiAppRegistrationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var headerSK = HttpContext.Request?.Headers[nameof(_appSettings.AppHeaderSecretKey)];

                if (!headerSK.HasValue || !headerSK.Equals(_appSettings.AppHeaderSecretKey))
                {
                    ModelState.AddModelError("", "Invalid request.");
                    return ValidationResult(ModelState);
                }

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

                        if (!userSaveResult.Succeeded)
                        {
                            throw new IdentityValidationException(userSaveResult.Errors);
                        };

                        // Add New User Role
                        var userRoleName = ConstantsUserRole.AppUser;
                        var role = await _roleManager.FindByNameAsync(userRoleName);

                        if (role == null)
                        {
                            throw new NotFoundException(nameof(ApplicationRole), userRoleName);
                        }

                        var roleSaveResult = await _userManager.AddToRoleAsync(user, role.Name);

                        if (!roleSaveResult.Succeeded)
                        {
                            throw new IdentityValidationException(roleSaveResult.Errors);
                        };

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

                var authUser = _mapper.Map<ApplicationUser, ApiAuthenticateUserModel>(user);

                // authentication successful so generate jwt token
                authUser.Token = await this.BuildToken(user);

                return OkResult(authUser);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpPost("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody] ApiAppChangePasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var header = HttpContext.Request?.Headers[nameof(_appSettings.AppHeaderSecretKey)];

                if (!header.HasValue || !header.Equals(_appSettings.AppHeaderSecretKey))
                {
                    ModelState.AddModelError("", "Invalid request.");
                    return ValidationResult(ModelState);
                }

                var user = await this._userManager.FindByNameAsync(model.PhoneNumber);
                if (user == null)
                {
                    ModelState.AddModelError("", "Phone Number or current password is invalid.");

                    return ValidationResult(ModelState);
                }

                var isValidUser = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isValidUser)
                {
                    ModelState.AddModelError("", "Phone Number or current password is invalid.");
                    return ValidationResult(ModelState);
                }
                user.PasswordChangedCount = user.PasswordChangedCount + 1;
                await _userManager.UpdateAsync(user);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!result.Succeeded)
                {
                    throw new IdentityValidationException(result.Errors);
                };

                return OkResult(true);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        private async Task<string> BuildToken(ApplicationUser user)
        {
            // authentication successful so generate jwt token
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
                Expires = DateTime.UtcNow.AddHours(this._appSettings.TokenExpiresHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}