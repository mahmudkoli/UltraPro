using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Services.Interfaces;
using UltraPro.API.Controllers.Common;
using UltraPro.API.Core;
using UltraPro.API.Models;
using UltraPro.API.Models.IdentityModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace UltraPro.API.Controllers.Users
{
    [Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/users")]
    public class UsersController : BaseController
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public UsersController(
            IApplicationUserService applicationUserService,
            IOptionsSnapshot<AppSettings> appOptions,
            IMapper mapper)
        {
            this._applicationUserService = applicationUserService;
            this._appSettings = appOptions.Value;
            this._mapper = mapper;
        }

        [HttpGet]
        [Authorize(Permissions.Users.ListView)]
        public async Task<IActionResult> Get([FromQuery] UserQuery query)
        {
            try
            {
                var result = await _applicationUserService.GetAllAsync(query);
                var queryResult = _mapper.Map<QueryResult<ApplicationUser>, QueryResult<UserModel>>(result);
                return OkResult(queryResult);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpGet("except-app-users")]
        [Authorize(Permissions.Users.ListView)]
        public async Task<IActionResult> GetExceptAppUsers([FromQuery] UserQuery query)
        {
            try
            {
                var result = await _applicationUserService.GetAllExceptAppUsersAsync(query);
                var queryResult = _mapper.Map<QueryResult<ApplicationUser>, QueryResult<UserModel>>(result);
                return OkResult(queryResult);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpGet("app-users")]
        [Authorize(Permissions.Users.ListView)]
        public async Task<IActionResult> GetAppUsers([FromQuery] UserQuery query)
        {
            try
            {
                var result = await _applicationUserService.GetAllAppUsersAsync(query);
                var queryResult = _mapper.Map<QueryResult<ApplicationUser>, QueryResult<UserModel>>(result);
                return OkResult(queryResult);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Permissions.Users.DetailsView)]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var user = await _applicationUserService.GetByIdAsync(id);
                var result = _mapper.Map<ApplicationUser, UserModel>(user);
                return OkResult(result);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpPost]
        [Authorize(Permissions.Users.Create)]
        public async Task<IActionResult> Create([FromBody] SaveUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var user = _mapper.Map<SaveUserModel, ApplicationUser>(model);
                var result = await _applicationUserService.AddAsync(user, model.UserRoleId, _appSettings.UserDefaultPassword);
                return OkResult(result);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Permissions.Users.Edit)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var user = _mapper.Map<SaveUserModel, ApplicationUser>(model);
                await _applicationUserService.UpdateAsync(user, model.UserRoleId);
                return OkResult(true);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Permissions.Users.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _applicationUserService.DeleteAsync(id);
                return OkResult(true);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpPost("activeInactive/{id}")]
        public async Task<IActionResult> ActiveInactive(Guid id)
        {
            try
            {
                await _applicationUserService.ActiveInactiveAsync(id);
                return OkResult(true);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            try
            {
                var result = await _applicationUserService.GetAllForSelectAsync();
                return OkResult(result);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpGet("select/except-app-users")]
        public async Task<IActionResult> GetSelectExceptAppUsers()
        {
            try
            {
                var result = await _applicationUserService.GetAllExceptAppUsersForSelectAsync();
                return OkResult(result);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpGet("select/app-users")]
        public async Task<IActionResult> GetSelectAppUsers()
        {
            try
            {
                var result = await _applicationUserService.GetAllAppUsersForSelectAsync();
                return OkResult(result);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }
    }
}
