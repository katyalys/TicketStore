using Catalog.Application.Dtos.BasketDtos;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;

namespace Catalog.Application.Interfaces
{
    public interface IBasketService
    {
        Task<Result<BasketDto>> AddBasketTicketAsync(int ticketId, string userId);
        Task<Result<BasketDto>> GetBasketAsync(string userId);
        Task<Result<BasketDto>> DeleteFromBasketTicketAsync(int ticketId, string userId);
        Task<Result> DeleteBasketAsync(string userId);
        Task<Result<Dictionary<string, Basket>>> GetAllBasketsAsync();
    }
}
