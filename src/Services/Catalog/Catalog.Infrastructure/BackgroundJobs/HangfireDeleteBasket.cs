using Catalog.Application.Interfaces;
using Hangfire;

namespace Catalog.Infrastructure.BackgroundJobs
{
    public static class HangfireDeleteBasket
    {
        public static void DeleteBasket(string expiredKey)
        {
            BackgroundJob.Enqueue<IBackgroundJobsService>(x => x.DeleteBasketAsync(expiredKey));
        }
    }
}
