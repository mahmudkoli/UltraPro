using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.API.Models;
using UltraPro.Entities;

namespace UltraPro.API.Services
{
    public interface IAuthenticationService
    {
        Task<ApiAuthenticateUserModel> AuthenticateTokenAsync(ApplicationUser user, HttpContext context, HttpRequest request, HttpResponse response);
        Task<ApiAuthenticateUserModel> RefreshTokenAsync(HttpContext context, HttpRequest request, HttpResponse response);
        Task<bool> RevokeTokenAsync(ApiAppRevokeTokenModel model, HttpContext context, HttpRequest request);
    }
}
