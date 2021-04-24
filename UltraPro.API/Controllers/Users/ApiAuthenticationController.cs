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
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace UltraPro.API.Controllers.Users
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/auth")]
    public class ApiAuthenticationController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailSender _emailSender;

        public ApiAuthenticationController(
            IOptionsSnapshot<AppSettings> options,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IEmailSender emailSender,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _appSettings = options.Value;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiLoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var user = await this._userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", "UserName or password is invalid.");
                    return ValidationResult(ModelState);
                }

                var isValidUser = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isValidUser)
                {
                    user.AccessFailedCount += 1;
                    var result = await _userManager.UpdateAsync(user);

                    ModelState.AddModelError("", "UserName or password is invalid.");
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
        [HttpGet("forgot-password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var user = await this._userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email is invalid.");
                    return ValidationResult(ModelState);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var callbackUrl = this._appSettings.ResetPasswordUrl +
                                    "?userId=" + user.Id + "&token=" + token;

                //email = "mahmud.koli@brainstation23.com";
                await _emailSender.SendEmailAsync(email, "Forgot Password",
                        $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return OkResult(true);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }
        
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ApiResetPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);
                    
                var user = await this._userManager.FindByIdAsync(model.UserId.ToString());
                if (user == null)
                {
                    ModelState.AddModelError("", "User Id or token is invalid.");
                    return ValidationResult(ModelState);
                }

                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "User Id or token is invalid.");
                    return ValidationResult(ModelState);
                }
                
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