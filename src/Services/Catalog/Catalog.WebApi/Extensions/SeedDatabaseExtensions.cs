using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.WebApi.Extensions
{
	public static class SeedDatabaseExtensions
	{
		public static async Task<WebApplication> UseDatabaseSeed(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;

			await SeedDatabaseAsync(services);

			return app;
		}

		private static async Task SeedDatabaseAsync(IServiceProvider services)
		{
			var context = services.GetRequiredService<CatalogContext>();
			await context.Database.MigrateAsync();

			await CatalogContextSeed.SeedAsync(context);
		}
	}
}
