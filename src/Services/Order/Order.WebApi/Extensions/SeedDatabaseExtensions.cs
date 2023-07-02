using Microsoft.EntityFrameworkCore;
using Order.Infrastructure.Data;

namespace Order.WebApi.Extensions
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
            var context = services.GetRequiredService<OrderContext>();
            await context.Database.MigrateAsync();

            await OrderContextSeed.SeedAsync(context);
        }
    }
}
