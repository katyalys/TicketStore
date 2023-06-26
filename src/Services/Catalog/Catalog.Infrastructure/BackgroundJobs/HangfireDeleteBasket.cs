using Catalog.Application.Interfaces;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.BackgroundJobs
{
    public static class HangfireDeleteBasket
    {
        public static void DeleteBasket(string expiredKey)
        {
            BackgroundJob.Enqueue<IBackgroundJobsService>(x => x.DeleteBasket(expiredKey));
        }
    }
}
