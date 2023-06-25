using Catalog.Domain.Entities;
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
        Task AddBasketUpdate(Ticket ticket, string userId);
        Task DeleteBasket(string userId);
        Task UpdateDeletedConcerts();
    }
}
