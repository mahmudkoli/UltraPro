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
using UltraPro.API.Services;

namespace UltraPro.API.Controllers.Users
{
    [Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/auth")]
    public class ApiAuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public ApiAuthenticationController(
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiLoginModel model)
        {
            var user = await _authenticationService.AuthenticatePortalAsync(model);
            var authUser = await _authenticationService.AuthenticateTokenAsync(user, HttpContext, Request, Response);
            return OkResult(authUser);
        }

        [AllowAnonymous]
        [HttpGet("forgot-password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var response = await _authenticationService.ForgotPasswordPortalAsync(email);
            return OkResult(response);
        }
        
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ApiResetPasswordModel model)
        {
            var response = await _authenticationService.ResetPasswordPortalAsync(model);
            return OkResult(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var response = await _authenticationService.RefreshTokenAsync(HttpContext, Request, Response);
            return OkResult(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeTokenAsync([FromBody] ApiAppRevokeTokenModel model)
        {
            var response = await _authenticationService.RevokeTokenAsync(model, HttpContext, Request);
            return OkResult(response);
        }
    }
}