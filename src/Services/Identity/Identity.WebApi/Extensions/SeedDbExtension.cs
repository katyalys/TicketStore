using Identity.Domain;
using Identity.Infrastructure;
using Identity.Infrastructure.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.WebApi.Extensions
{
	public static class SeedDbExtensions
	{
		//public static async Task<IHost> UseDatabaseSeed(this IHost app)
		//{
		//	using var scope = app.Services.CreateScope();
		//	var services = scope.ServiceProvider;

		//	await SeedDatabaseAsync(services);

		//	return app;
		//}

		//private static async Task SeedDatabaseAsync(IServiceProvider services)
		//{
		//	var identityContext = services.GetRequiredService<IdentityDbContext>(); 
		//	await identityContext.Database.MigrateAsync();
		//	IdentitySeed.SeedData(identityContext);
		//}
		public static async Task<WebApplication> UseDatabaseSeed(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;

			await SeedIdentityAsync(services);

			return app;
		}

		private static async Task SeedIdentityAsync(IServiceProvider services)
		{
			var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<IdentityUser>>();
			var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<IdentityRole>>();

			var identityContext = services.GetRequiredService<IdentityDbContext>();
			await identityContext.Database.MigrateAsync();

			await IdentitySeed.SeedDataAsync(userManager, roleManager);
		}

	}
}
