using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Hangfire;

namespace Catalog.Infrastructure.BackgroundJobs
{
    public class HangfireUpdateBasket
    {
        public static void UpdateBasket(Ticket ticket, string userId)
        {
            BackgroundJob.Enqueue<IBackgroundJobsService>(
                            x => x.AddBasketUpdateAsync(ticket, userId));
        }
    }
}
