using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UltraPro.API.Core;
using UltraPro.API.Models;
using UltraPro.Common.Services;
using UltraPro.Entities;
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

        public AuthenticationService(
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            IRefreshTokenService refreshTokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<AppSettings> appSettings,
            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
            _refreshTokenService = refreshTokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
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
