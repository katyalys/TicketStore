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

        public BasketService(IRedisRepository redisRepository, IMapper mapper, IUnitOfWork unitOfWork, CatalogContext context)
        {
            _mapper = mapper;
            _redisRepository = redisRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<BasketDto> AddBasketTicketAsync(int ticketId, string userId)
        {
            var spec = new TicketAddToBasket(ticketId);
            var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);

            //var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Cant add ticket to basket");
            }

            ticket.StatusId = (int)StatusTypes.Book + 1;
            ticket.CustomerId = userId;
            _unitOfWork.Repository<Ticket>().Update(ticket);
            await _unitOfWork.Complete();

            Basket? basket = await _redisRepository.Get<Basket>(userId);
            if (basket == null)
            {
                basket = new Basket();
                basket.TicketIds = new List<int>();
            }
            basket.TicketIds.Add(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);
           // basket.TimeToBuy = TimeSpan.FromMinutes(20);
            await _redisRepository.Add(userId, basket, TimeSpan.FromMinutes(20));

            return await GetBasketAsync(userId);
        }

        public async Task<BasketDto> GetBasketAsync(string userId)
        {
            var expire = await GetExpiredTicketKeys(userId);
            if (expire == null)
            {
                throw new Exception("No items in basket");
            }

            var keyExists = await _redisRepository.Exists(userId, TimeSpan.FromMinutes(20));
            Basket basket = await _redisRepository.Get<Basket>(userId);
            var basketDto = _mapper.Map<BasketDto>(basket);
            basketDto.TimeToBuy = (TimeSpan)expire;
            basketDto.Tickets = new List<TicketDto>();

            foreach (var ticketId in basket.TicketIds) 
            {
                var spec = new TicketsInfo(ticketId);
                var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);
                var ticketDto = _mapper.Map<TicketDto>(ticket);
                basketDto.Tickets.Add(ticketDto);
            }
            
            return basketDto;
        }

        public async Task<BasketDto> DeleteFromBasketTicket(int ticketId, string userId)
        {
            Basket basket = await _redisRepository.Get<Basket>(userId);
            basket.TicketIds.Remove(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);

            var spec = new TicketDeleteFromBasket(userId, ticketId);
            var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);
            ticket.StatusId = (int)StatusTypes.Free + 1;
            ticket.CustomerId = null;
            _unitOfWork.Repository<Ticket>().Update(ticket);
            await _unitOfWork.Complete();

            var updatedBasket = await _redisRepository.Update<Basket>(userId, basket, TimeSpan.FromMinutes(20));

            return await GetBasketAsync(userId);
        }

        public async Task DeleteBasketAsync(string userId)
        {
            var spec = new TicketDeleteFromBasket(userId);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);
            foreach (var ticket in tickets)
            {
                ticket.StatusId = (int)StatusTypes.Free;
                ticket.CustomerId = null;
                _unitOfWork.Repository<Ticket>().Update(ticket);
            }
            await _unitOfWork.Complete();

            var removed = await _redisRepository.Remove(userId);
            if (!removed)
            {
                throw new Exception("No items in basket");
            }
        }

        public async Task<Dictionary<string, Basket>> GetAllBaskets()
        {
            return await _redisRepository.GetList<Basket>();
        }

        private async Task<TimeSpan?> GetExpiredTicketKeys(string userId)
        {
            var timeToLive = _redisRepository.TimeToExpire(userId);

            if (timeToLive == null/*timeToLive.HasValue && timeToLive.Value < TimeSpan.Zero*/)
            {
                var spec = new TicketDeleteFromBasket(userId);
                var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

                foreach (var ticket in tickets) 
                {
                    ticket.StatusId = (int)StatusTypes.Free + 1;
                    ticket.CustomerId = null;
                    _unitOfWork.Repository<Ticket>().Update(ticket);
                }
                await _unitOfWork.Complete();
            }
            return timeToLive;
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
