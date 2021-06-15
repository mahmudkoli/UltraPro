using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Services.Interfaces;
using UltraPro.API.Controllers.Common;
using UltraPro.API.Models;
using UltraPro.API.Models.IdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UltraPro.API.Controllers.Users
{
    [Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/roles")]
    public class RolesController : BaseController
    {
        private IApplicationRoleService _applicationRoleService;
        private readonly IMapper _mapper;

        public RolesController(
            IApplicationRoleService applicationRoleService,
            IMapper mapper)
        {
            this._applicationRoleService = applicationRoleService;
            this._mapper = mapper;
        }

        [HttpGet]
        [Authorize(Permissions.UserRoles.ListView)]
        public async Task<IActionResult> Get([FromQuery] UserRoleQuery query)
        {
            var result = await _applicationRoleService.GetAllAsync(query);
            var queryResult = _mapper.Map<QueryResult<ApplicationRole>, QueryResult<UserRoleModel>>(result);
            return OkResult(queryResult);
        }

        [HttpGet("{id}")]
        [Authorize(Permissions.UserRoles.DetailsView)]
        public async Task<IActionResult> Get(Guid id)
        {
            var role = await _applicationRoleService.GetByIdAsync(id);
            var result = _mapper.Map<ApplicationRole, UserRoleModel>(role.ApplicationRole);
            result.Permissions = role.Permissions;
            return OkResult(result);
        }

        [HttpPost]
        [Authorize(Permissions.UserRoles.Create)]
        public async Task<IActionResult> Create([FromBody] SaveUserRoleModel model)
        {
            var role = _mapper.Map<SaveUserRoleModel, ApplicationRole>(model);
            var result = await _applicationRoleService.AddAsync(role, model.Permissions);
            return OkResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Permissions.UserRoles.Edit)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveUserRoleModel model)
        {
            var role = _mapper.Map<SaveUserRoleModel, ApplicationRole>(model);
            var result = await _applicationRoleService.UpdateAsync(role, model.Permissions);
            return OkResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Permissions.UserRoles.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _applicationRoleService.DeleteAsync(id);
            return OkResult(result);
        }

        [HttpPost("activeInactive/{id}")]
        public async Task<IActionResult> ActiveInactive(Guid id)
        {
            var result = await _applicationRoleService.ActiveInactiveAsync(id);
            return OkResult(result);
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var result = await _applicationRoleService.GetAllForSelectAsync();
            return OkResult(result);
        }
    }
}
