using Catalog.Application.Dtos.BasketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Catalog.Infrastructure.Services;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.BackgroundJobs
{
    public static class UpdateCatalogDbJob
    {

        public static void Start(int ticketId, string userId)
        {
            BackgroundJob.Enqueue<IBackgroundJobsService>(
                x => x.AddBasketUpdate(ticketId, userId));

            RecurringJobOptions options = new()
            {
                TimeZone = TimeZoneInfo.Local,
            };
            RecurringJob.AddOrUpdate<IBackgroundJobsService>("job-id",
                x => x.UpdateDeletedConcerts(), Cron.Daily, options);


        }
        
       
    }
}
