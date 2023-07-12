using Catalog.Domain.Entities;

namespace Catalog.Application.Interfaces
{
    public interface IBackgroundJobsService
    {
        Task AddBasketUpdateAsync(Ticket ticket, string userId);
        Task DeleteBasketAsync(string userId);
        Task UpdateDeletedConcertsAsync();
    }
}
