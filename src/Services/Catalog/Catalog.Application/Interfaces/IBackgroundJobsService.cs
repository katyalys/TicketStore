using Catalog.Domain.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface IBackgroundJobsService
    {
        Task<Result> AddBasketUpdate(int ticketId, string userId);
        Task UpdateDeletedConcerts();
    }
}
