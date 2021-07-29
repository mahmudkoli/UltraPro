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
using UltraPro.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace UltraPro.API.Controllers.Users
{
    //[Authorize(Roles = ConstantsUserRole.AppUser)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/test")]
    public class TestController : BaseController
    {
        private readonly ILogger<TestController> _logger;
        private readonly IApplicationUserService _applicationUserService;
        private IApplicationRoleService _applicationRoleService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public TestController(
            ILogger<TestController> logger,
            IApplicationUserService applicationUserService,
            IApplicationRoleService applicationRoleService,
            IOptionsSnapshot<AppSettings> appOptions,
            IMapper mapper)
        {
            this._logger = logger;
            this._applicationUserService = applicationUserService;
            this._applicationRoleService = applicationRoleService;
            this._appSettings = appOptions.Value;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Test Information Log Issue.");
            _logger.LogWarning("Test Warning Log Issue.");
            _logger.LogError("Test Error Log Issue.");
            throw new Exception("Test Exception Log Issue.");
            return OkResult(new object());
        }
    }
}
