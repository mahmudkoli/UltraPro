using UltraPro.Services.Interfaces;
using UltraPro.API.Controllers.Common;
using UltraPro.API.Core;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UltraPro.Common.Constants;
using UltraPro.API.Models.IdentityModels;
using System.Threading.Tasks;
using System;
using UltraPro.Entities.NotMapped;
using UltraPro.Entities;
using UltraPro.API.Models;
using UltraPro.Services.Exceptions;

namespace UltraPro.API.Controllers.Users
{
    //[Authorize(Roles = ConstantsUserRole.AppUser)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/test")]
    public class TestController : BaseController
    {
        private readonly IApplicationUserService _applicationUserService;
        private IApplicationRoleService _applicationRoleService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public TestController(
            IApplicationUserService applicationUserService,
            IApplicationRoleService applicationRoleService,
            IOptionsSnapshot<AppSettings> appOptions,
            IMapper mapper)
        {
            this._applicationUserService = applicationUserService;
            this._applicationRoleService = applicationRoleService;
            this._appSettings = appOptions.Value;
            this._mapper = mapper;
        }

        [HttpGet]
        //[Authorize(Permissions.UserRoles.ListView)]
        public async Task<IActionResult> Get([FromQuery] UserRoleQuery query)
        {
            throw new NotFoundException("ApplicationRole", "Admin");

            try
            {
                throw new NotFoundException("ApplicationRole", "Admin");
                var result = await _applicationRoleService.GetAllAsync(query);
                var queryResult = _mapper.Map<QueryResult<ApplicationRole>, QueryResult<UserRoleModel>>(result);
                return OkResult(queryResult);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpGet("{id}")]
        //[Authorize(Permissions.UserRoles.DetailsView)]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var role = await _applicationRoleService.GetByIdAsync(id);
                var result = _mapper.Map<ApplicationRole, UserRoleModel>(role.ApplicationRole);
                result.Permissions = role.Permissions;
                return OkResult(result);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpPost]
        //[Authorize(Permissions.UserRoles.Create)]
        public async Task<IActionResult> Create([FromBody] SaveUserRoleModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var role = _mapper.Map<SaveUserRoleModel, ApplicationRole>(model);
                var result = await _applicationRoleService.AddAsync(role, model.Permissions);
                return OkResult(result);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Permissions.UserRoles.Edit)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveUserRoleModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationResult(ModelState);

                var role = _mapper.Map<SaveUserRoleModel, ApplicationRole>(model);
                await _applicationRoleService.UpdateAsync(role, model.Permissions);
                return OkResult(true);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Permissions.UserRoles.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _applicationRoleService.DeleteAsync(id);
                return OkResult(true);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }
    }
}
