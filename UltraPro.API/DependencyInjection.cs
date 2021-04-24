using UltraPro.API.Core;
using Microsoft.Extensions.DependencyInjection;
using UltraPro.Services.Interfaces;
using UltraPro.Services.Implements;

namespace UltraPro.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<IApplicationUserService, ApplicationUserService>();
            services.AddTransient<IApplicationRoleService, ApplicationRoleService>();

            return services;
        }
    }
}
