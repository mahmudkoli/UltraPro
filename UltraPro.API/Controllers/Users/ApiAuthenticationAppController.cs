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
using Microsoft.AspNetCore.Http;
using UltraPro.API.Services;

namespace UltraPro.API.Controllers.Users
{
    //[AllowAnonymous]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/app/auth")]
    public class ApiAuthenticationAppController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ApiAuthenticationAppController(
            IOptionsSnapshot<AppSettings> options,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IAuthenticationService authenticationService,
            IDateTime dateTime,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authenticationService = authenticationService;
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

                var authUser = await _authenticationService.AuthenticateTokenAsync(user, HttpContext, Request, Response);

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

                var authUser = await _authenticationService.AuthenticateTokenAsync(user, HttpContext, Request, Response);

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

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            try
            {
                var response = await _authenticationService.RefreshTokenAsync(HttpContext,Request, Response);

                return OkResult(response);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeTokenAsync([FromBody] ApiAppRevokeTokenModel model)
        {
            try 
            {
                var response = await _authenticationService.RevokeTokenAsync(model, HttpContext, Request);

                return OkResult(response);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }
    }
}