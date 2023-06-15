using AutoMapper;
using Catalog.Application.Dtos.BasketDtos;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace Catalog.Infrastructure.Services
{
    public class BasketService : IBasketService
    {
        private readonly IDatabase _database;
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        public BasketService(IConnectionMultiplexer redis, IMapper mapper)
        {
            _mapper = mapper;
            _database = redis.GetDatabase();
        }

        public async Task<BasketDto> AddBasketTicketAsync(int ticketId, string userId)
        {
            var data = await _database.StringGetAsync(userId);

            if (data.IsNull)
            {
                throw new Exception("No tickets in basket");
            }

            var basket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Basket>(data);
            basket.TicketIds.Add(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);
            basket.TimeToBuy = TimeSpan.FromMinutes(20);
            await _database.StringSetAsync(userId, JsonSerializer.Serialize(basket),
                TimeSpan.FromMinutes(20));

            return await GetBasketAsync(userId);
        }

        public async Task<BasketDto> GetBasketAsync(string userId)
        {
            var data = await _database.StringGetAsync(userId);
            var basket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Basket>(data);
            var basketDto = _mapper.Map<BasketDto>(basket);

            foreach (var ticketId in basket.TicketIds) 
            {
                var ticket = _context.Tickets.Include(s => s.Sector).Include(s => s.Concert).ThenInclude(s => s.Place).FirstOrDefault(t => t.Id == ticketId);
                var ticketDto = _mapper.Map<TicketDto>(ticket);
                basketDto.Tickets.Add(ticketDto);
            }
           
            return basketDto;
        }

        public async Task<BasketDto> DeleteFromBasketTicket(int ticketId, string userId)
        {
            var data = await _database.StringGetAsync(userId);
            var basket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Basket>(data);
            basket.TicketIds.Remove(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);
            await _database.StringSetAsync(userId, JsonSerializer.Serialize(basket),
                TimeSpan.FromMinutes(20));

            return await GetBasketAsync(userId);
        }

        public async Task<bool> DeleteBasketAsync(string userId)
        {
            return await _database.KeyDeleteAsync(userId);
        }

        private decimal CalculateTotalPrice(List<int> ticketIds)
        {
            decimal totalPrice = 0;

            if (ticketIds != null && ticketIds.Count > 0)
            {
                foreach (var ticketId in ticketIds)
                {
                    var ticket = _context.Tickets.Include(s => s.Sector).FirstOrDefault(t => t.Id == ticketId);
                    if (ticket != null)
                    {
                        totalPrice += ticket.Sector.Price;
                    }
                }
            }

            return totalPrice;
        }
    }
}
