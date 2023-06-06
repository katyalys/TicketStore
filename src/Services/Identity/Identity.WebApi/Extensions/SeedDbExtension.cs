using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.WebApi.Extensions
{
	public static class SeedDbExtensions
	{
		public static async Task<WebApplication> UseDatabaseSeed(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;

			await SeedIdentityAsync(services);

			return app;
		}

		private static async Task SeedIdentityAsync(IServiceProvider services)
		{
			var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
			var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

			var identityContext = services.GetRequiredService<IdentityDbContext>();
			await identityContext.Database.MigrateAsync();

			await IdentitySeed.SeedDataAsync(userManager, roleManager);
		}

	}
}
