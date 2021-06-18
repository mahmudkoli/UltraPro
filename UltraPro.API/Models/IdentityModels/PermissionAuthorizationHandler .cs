using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UltraPro.Common.Constants;
using UltraPro.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UltraPro.API.Models.IdentityModels
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        UserManager<ApplicationUser> _userManager;
        RoleManager<ApplicationRole> _roleManager;

        public PermissionAuthorizationHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }

            // var authFilterContext = context.Resource as AuthorizationFilterContext;
            // if (authFilterContext == null)
            // {
            //     return;
            // }

            // Get all the roles the user belongs to and check if any of the roles has the permission required
            // for the authorization to succeed.
            var claimsIdentity = context.User.Identity as ClaimsIdentity;
            if (!claimsIdentity.IsAuthenticated) return;
            //var tokenCreationDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Int64.Parse(context.User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Iat).Value));
            //var tokenExpirationDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Int64.Parse(context.User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Exp).Value));
            //var user = await _userManager.GetUserAsync(context.User);
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return;
            var user = await _userManager.FindByIdAsync(userId);
            var userRoleNames = await _userManager.GetRolesAsync(user);
            var userRoles = _roleManager.Roles.Where(x => userRoleNames.Contains(x.Name));
            //var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var guidUserId = Guid.Empty;
            //if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out guidUserId)) return;
            //var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();
            //query = query.IgnoreQueryFilters();
            //var user = await query.FirstOrDefaultAsync(x => x.Id == guidUserId);
            //if (user == null) return;
            //var userRoles = user.UserRoles.Select(x => x.Role).ToList();

            foreach (var role in userRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                #region for Super Admin
                var isSuperAdmin = roleClaims.Any(x => x.Type == CustomClaimType.Permission &&
                                                        x.Value == Permissions.SuperAdmin &&
                                                        x.Issuer == "LOCAL AUTHORITY");
                if (isSuperAdmin)
                {
                    context.Succeed(requirement);
                    return;
                }
                #endregion

                var permissions = roleClaims.Where(x => x.Type == CustomClaimType.Permission &&
                                                        x.Value == requirement.Permission &&
                                                        x.Issuer == "LOCAL AUTHORITY")
                                            .Select(x => x.Value);

                if (permissions.Any())
                {
                    context.Succeed(requirement);
                    return;
                }
            }
        }
    }
}