using Identity.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Identity.Domain.Enums.EnumAttributes;
using System.Security.Claims;

namespace Identity.Infrastructure.Data
{
	public class IdentitySeed
	{
		public static async Task SeedDataAsync(Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager, Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager) {

            if (roleManager.Roles.Any())
                return;

            var passwordHasher = new PasswordHasher<User>();
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
            await userManager.AddClaimsAsync(user, new List<Claim>{
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim(ClaimTypes.Name, admin.UserName),
                new Claim(ClaimTypes.Email, admin.Email),
            });

            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRoleAsync(admin, "Admin");
            await userManager.AddClaimsAsync(admin, new List<Claim>{
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Name, admin.UserName),
                new Claim(ClaimTypes.Email, admin.Email),
            });

        }

        //public static void SeedData(IdentityDbContext identityDbContext)
        //{
        //	if (!identityDbContext.User.Any())
        //	{
        //		var passwordHasher = new PasswordHasher<User>();

        //		var events = new List<User>
        //		{
        //			new User
        //			{
        //				Email = "admin@gmail.com",
        //				PhoneNumber = "+375296880051",
        //				UserName = "admin",
        //				NormalizedUserName = "admin".Normalize().ToUpperInvariant(),
        //				NormalizedEmail = "admin@gmail.com".Normalize().ToUpperInvariant(),
        //				Password = passwordHasher.HashPassword(null, "Admin123"),
        //				PasswordHash = passwordHasher.HashPassword(null, "Admin123"),
        //				IsAdmin = IsAdminEnum.Admin,
        //				IsDeleted = IsDeletedEnum.NotDeleted,
        //			},

        //			new User
        //			{
        //				Email = "katya@gmail.com",
        //				PhoneNumber = "+375296580051",
        //				UserName = "katya",
        //				NormalizedUserName = "katya".Normalize().ToUpperInvariant(),
        //				NormalizedEmail = "katya@gmail.com".Normalize().ToUpperInvariant(),
        //				Password = passwordHasher.HashPassword(null, "Katya123"),
        //				PasswordHash = passwordHasher.HashPassword(null, "Katya123"),
        //				IsAdmin = IsAdminEnum.NotAdmin,
        //				IsDeleted = IsDeletedEnum.NotDeleted,
        //			},

        //		};
        //		identityDbContext.User.AddRange(events);
        //		identityDbContext.SaveChanges();
        //	}
        //}
    }
}
