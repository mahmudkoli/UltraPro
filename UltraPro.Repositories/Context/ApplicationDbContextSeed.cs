using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Constants;
using UltraPro.Common.Enums;
using UltraPro.Entities;

namespace UltraPro.Repositories.Context
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            var password = "12345";

            var superAdminRole = new ApplicationRole { Name = ConstantsUserRole.SuperAdmin, Status = EnumApplicationRoleStatus.SuperAdmin };
            var adminRole = new ApplicationRole { Name = ConstantsUserRole.Admin, Status = EnumApplicationRoleStatus.GeneralUser };
            var generalRole = new ApplicationRole { Name = ConstantsUserRole.GeneralUser, Status = EnumApplicationRoleStatus.GeneralUser };
            var appRole = new ApplicationRole { Name = ConstantsUserRole.AppUser, Status = EnumApplicationRoleStatus.GeneralUser };

            if (roleManager.Roles.All(r => r.Name != superAdminRole.Name))
                await roleManager.CreateAsync(superAdminRole);
            if (roleManager.Roles.All(r => r.Name != adminRole.Name))
                await roleManager.CreateAsync(adminRole);
            if (roleManager.Roles.All(r => r.Name != generalRole.Name))
                await roleManager.CreateAsync(generalRole);
            if (roleManager.Roles.All(r => r.Name != appRole.Name))
                await roleManager.CreateAsync(appRole);

            var adminUser = new ApplicationUser { UserName = "admin", Email = "admin@gmail.com", Status = EnumApplicationUserStatus.SuperAdmin };
            var devUser = new ApplicationUser { UserName = "dev", Email = "dev@gmail.com", Status = EnumApplicationUserStatus.SuperAdmin };

            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                await userManager.CreateAsync(adminUser, password);
                await userManager.AddToRolesAsync(adminUser, new[] { superAdminRole.Name });
            }
            if (userManager.Users.All(u => u.UserName != devUser.UserName))
            {
                await userManager.CreateAsync(devUser, password);
                await userManager.AddToRolesAsync(devUser, new[] { superAdminRole.Name });
            }
            if (!(await roleManager.GetClaimsAsync(adminRole))
                    .Any(c => c.Type == ConstantsRolePermission.Type && c.Value == ConstantsRolePermission.Value))
                await roleManager.AddClaimAsync(adminRole, new Claim(ConstantsRolePermission.Type, ConstantsRolePermission.Value));
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.Products.Any())
            {
                context.Products.Add(new Product
                {
                    Name = "Berger",
                    Code = "B01",
                    Description = "None Color",
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
