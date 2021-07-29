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
using UltraPro.Common.Exceptions;
using UltraPro.Common.Constants;
using UltraPro.Services.Interfaces;
using UltraPro.Common.Enums;
using System.Transactions;
using UltraPro.Common.Services;
using Microsoft.AspNetCore.Http;
using UltraPro.API.Services;

namespace UltraPro.API.Controllers.Users
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/app/auth")]
    public class ApiAuthenticationAppController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public ApiAuthenticationAppController(
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiAppLoginModel model)
        {
            var user = await _authenticationService.AuthenticateAppAsync(model, HttpContext);
            var authUser = await _authenticationService.AuthenticateTokenAsync(user, HttpContext, Request, Response);
            return OkResult(authUser);
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] ApiAppRegistrationModel model)
        {
            var user = await _authenticationService.RegistrationAppAsync(model, HttpContext);
            var authUser = await _authenticationService.AuthenticateTokenAsync(user, HttpContext, Request, Response);
            return OkResult(authUser);
        }

        [AllowAnonymous]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ApiAppChangePasswordModel model)
        {
            var response = await _authenticationService.ChangePasswordAppAsync(model, HttpContext);
            return OkResult(true);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var response = await _authenticationService.RefreshTokenAsync(HttpContext,Request, Response);
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