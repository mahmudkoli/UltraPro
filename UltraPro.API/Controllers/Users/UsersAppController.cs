using UltraPro.Services.Interfaces;
using UltraPro.API.Controllers.Common;
using UltraPro.API.Core;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UltraPro.Common.Constants;

namespace UltraPro.API.Controllers.Users
{
    //[Authorize(Roles = ConstantsUserRole.AppUser)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/app/users")]
    public class UsersAppController : BaseController
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public UsersAppController(
            IApplicationUserService applicationUserService,
            IOptionsSnapshot<AppSettings> appOptions,
            IMapper mapper)
        {
            this._applicationUserService = applicationUserService;
            this._appSettings = appOptions.Value;
            this._mapper = mapper;
        }



    }
}
