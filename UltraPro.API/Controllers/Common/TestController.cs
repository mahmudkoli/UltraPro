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
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public TestController(
            IApplicationUserService applicationUserService,
            IOptionsSnapshot<AppSettings> appOptions,
            IMapper mapper)
        {
            this._applicationUserService = applicationUserService;
            this._appSettings = appOptions.Value;
            this._mapper = mapper;
        }

        [HttpGet]
        //[Authorize(Permissions.Users.ListView)]
        public async Task<IActionResult> Get()
        {
            throw new NotFoundException("test", "test");

            try
            {
                throw new ValidationException();
                throw new NotFoundException("test", "test");
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

    }
}
