using AutoMapper;
using Catalog.Application.Dtos.BasketDtos;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace Catalog.Infrastructure.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRedisRepository _redisRepository;
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BasketService(IRedisRepository redisRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _redisRepository = redisRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BasketDto> AddBasketTicketAsync(int ticketId, string userId)
        {
            var spec = new TicketAddToBasket(ticketId);
            if (spec == null)
            {
                throw new Exception("Cant add ticket to basket");
            }

            Basket basket = await _redisRepository.Get<Basket>(userId);
            basket.TicketIds.Add(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);
            basket.TimeToBuy = TimeSpan.FromMinutes(20);
            await _redisRepository.Add(userId, basket, TimeSpan.FromMinutes(20));

            var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);
            ticket.StatusId = (int)StatusTypes.Book;
            _unitOfWork.Repository<Ticket>().Update(ticket);
            await _unitOfWork.Complete();

            return await GetBasketAsync(userId);
        }

        public async Task<BasketDto> GetBasketAsync(string userId)
        {
            bool expire = await GetExpiredTicketKeys(userId);
            if (expire == true)
            {
                throw new Exception("No items in basket");
            }

            Basket basket = await _redisRepository.Get<Basket>(userId);
            var basketDto = _mapper.Map<BasketDto>(basket);

            foreach (var ticketId in basket.TicketIds) 
            {
                var spec = new TicketsInfo(ticketId, true);
                var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);
               // var ticket = _context.Tickets.Include(s => s.Sector).Include(s => s.Concert).ThenInclude(s => s.Place).FirstOrDefault(t => t.Id == ticketId);
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

        private async Task<bool> GetExpiredTicketKeys(string userId)
        {
            var timeToLive = _redisRepository.TimeToExpire(userId);

            if (timeToLive.HasValue && timeToLive.Value < TimeSpan.Zero)
            {
                var spec = new TicketDeleteFromBasket(userId);
                var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);
                foreach (var ticket in tickets) 
                {
                    ticket.StatusId = (int)StatusTypes.Free;
                    _unitOfWork.Repository<Ticket>().Update(ticket);
                }
                await _unitOfWork.Complete();
                return true;
            }
            return false;
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
