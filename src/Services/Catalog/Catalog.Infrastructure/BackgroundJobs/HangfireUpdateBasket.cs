using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.BackgroundJobs
{
    public class HangfireUpdateBasket
    {
        public static void UpdateBasket(Ticket ticket, string userId)
        {
            BackgroundJob.Enqueue<IBackgroundJobsService>(
                            x => x.AddBasketUpdate(ticket, userId));
        }
    }
}
