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
        Task<ApplicationUser> AuthenticateAppAsync(ApiAppLoginModel model, HttpContext context);
        Task<ApplicationUser> AuthenticatePortalAsync(ApiLoginModel model);
        Task<ApiAuthenticateUserModel> AuthenticateTokenAsync(ApplicationUser user, HttpContext context, HttpRequest request, HttpResponse response);
        Task<bool> ChangePasswordAppAsync(ApiAppChangePasswordModel model, HttpContext context);
        Task<bool> ForgotPasswordPortalAsync(string email);
        Task<ApiAuthenticateUserModel> RefreshTokenAsync(HttpContext context, HttpRequest request, HttpResponse response);
        Task<ApplicationUser> RegistrationAppAsync(ApiAppRegistrationModel model, HttpContext context);
        Task<bool> ResetPasswordPortalAsync(ApiResetPasswordModel model);
        Task<bool> RevokeTokenAsync(ApiAppRevokeTokenModel model, HttpContext context, HttpRequest request);
    }
}
