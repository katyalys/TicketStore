using Hangfire;
using Hangfire.SqlServer;

namespace Catalog.WebApi.Extensions
{
    public static class HangfireExtension
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, string connectionHangfireString)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionHangfireString, new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();

            return services;
        }
    }
}
