using Catalog.Application.Interfaces;
using Hangfire;

namespace Catalog.Infrastructure.BackgroundJobs
{
    public static class HangfireUpdateConcert
    {
        public static void HangfireUpdateConcerts()
        {
            RecurringJobOptions options = new()
            {
                TimeZone = TimeZoneInfo.Local,
            };
            RecurringJob.AddOrUpdate<IBackgroundJobsService>("job-1",
                x => x.UpdateDeletedConcerts(), Cron.Daily, options);
        }
    }
}
