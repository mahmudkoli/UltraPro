using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using UltraPro.Services.Interfaces;
using UltraPro.Common.Services;

namespace UltraPro.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out Guid value);
            UserId = value;
            IsAuthenticated = httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated??false;
        }

        public Guid UserId { get; }
        public bool IsAuthenticated { get; }
    }
}
