using Catalog.Application.Dtos.BasketDtos;
using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface IBasketService
    {
        Task<BasketDto> AddBasketTicketAsync(int ticketId, string userId);
        Task<BasketDto> GetBasketAsync(string userId);
        Task<BasketDto> DeleteFromBasketTicket(int ticketId, string userId);
        Task DeleteBasketAsync(string userId);
        Task<Dictionary<string, Basket>> GetAllBaskets();
    }
}
