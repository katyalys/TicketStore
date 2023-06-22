using Identity.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Identity.Infrastructure.Data
{
	public class IdentitySeed
	{
		public static async Task SeedDataAsync(Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager, Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager) {

            if (roleManager.Roles.Any())
            {
                return;
            }

            var passwordHasher = new PasswordHasher<IdentityUser>();
            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "Admin"
            });
            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "Customer"
            });

            var admin = new IdentityUser
            {
                Email = "admin@gmail.com",
                PhoneNumber = "+375296880051",
                UserName = "admin",
            };

            var user = new IdentityUser
            {
                Email = "katya@gmail.com",
                PhoneNumber = "+375296580051",
                UserName = "katya",
            };

            await userManager.CreateAsync(user, "Katya123");
            await userManager.AddToRoleAsync(user, "Customer");
            await userManager.SetLockoutEnabledAsync(user, false);
            await userManager.AddClaimsAsync(user, new List<Claim>{
                new Claim("role", "Customer"),
            });

            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRoleAsync(admin, "Admin");
            await userManager.SetLockoutEnabledAsync(admin, false);
            await userManager.AddClaimsAsync(admin, new List<Claim>{
                new Claim("role", "Admin"),
            });
        }
    }
}
