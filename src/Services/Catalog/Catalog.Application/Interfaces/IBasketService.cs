using Catalog.Application.Dtos.BasketDtos;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface IBasketService
    {
        Task<Result<BasketDto>> AddBasketTicketAsync(int ticketId, string userId);
        Task<Result<BasketDto>> GetBasketAsync(string userId);
        Task<Result<BasketDto>> DeleteFromBasketTicket(int ticketId, string userId);
        Task<Result> DeleteBasketAsync(string userId);
        Task<Result<Dictionary<string, Basket>>> GetAllBaskets();
    }
}
