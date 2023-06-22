using AutoMapper;
using Catalog.Application.Dtos.BasketDtos;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Catalog.Infrastructure.BackgroundJobs;
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

        public async Task<Result<BasketDto>> AddBasketTicketAsync(int ticketId, string userId)
        {
            UpdateCatalogDbJob.Start(ticketId, userId);
            //var spec = new TicketAddToBasket(ticketId);
            //var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);

            //if (ticket == null)
            //{
            //    return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, " Сant add ticket");
            //}

            //ticket.StatusId = (int)StatusTypes.Book + 1;
            //ticket.CustomerId = userId;
            //_unitOfWork.Repository<Ticket>().Update(ticket);
            //var updated = await _unitOfWork.Complete();

            //if (updated < 0)
            //{
            //    return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.WrongAction, "Value cant be updated in db");
            //}

            Basket? basket = await _redisRepository.Get<Basket>(userId);
            if (basket == null)
            {
                basket = new Basket();
                basket.TicketIds = new List<int>();
            }

            basket.TicketIds.Add(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);
            var result = await _redisRepository.Add(userId, basket, TimeSpan.FromMinutes(20));

            if (result == false)
            {
                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.WrongAction, "Value cant be added to redis");
            }

            return await GetBasketAsync(userId);
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string userId)
        {
            var expire = await GetExpiredTicketKeys(userId);
            if (expire.Value == null)
            {
                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, "No itmes in basket");
            }

            Basket basket = await _redisRepository.Get<Basket>(userId);
            var basketDto = _mapper.Map<BasketDto>(basket);
            basketDto.TimeToBuy = (TimeSpan)expire.Value;
            basketDto.Tickets = new List<TicketDto>();

            var ticketTasks = basket.TicketIds.Select(ticketId =>
            {
                var spec = new TicketsInfo(ticketId);
                return _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);
            });

            var tickets = await Task.WhenAll(ticketTasks);

            if (tickets.Any(ticket => ticket == null))
            {
                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, "Ticket not found");
            }

            basketDto.Tickets = tickets.Select(ticket => _mapper.Map<TicketDto>(ticket)).ToList();


            return new Result<BasketDto>()
            {
                Value = basketDto
            };
        }

        public async Task<Result<BasketDto>> DeleteFromBasketTicket(int ticketId, string userId)
        {
            Basket basket = await _redisRepository.Get<Basket>(userId);

            if (basket == null)
            {
                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, "No items in basket");
            }

            basket.TicketIds.Remove(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);

            var spec = new TicketDeleteFromBasket(userId, ticketId);
            var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);

            if (ticket == null)
            {
                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, "Ticket not found");
            }

            ticket.StatusId = (int)StatusTypes.Free + 1;
            ticket.CustomerId = null;
            _unitOfWork.Repository<Ticket>().Update(ticket);
            var updated = await _unitOfWork.Complete();

            if (updated < 0)
            {
                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.WrongAction, "Value cant be updated in db");
            }

            var updatedBasket = await _redisRepository.Update<Basket>(userId, basket, TimeSpan.FromMinutes(20));
            if (updatedBasket == false)
            {
                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.WrongAction, "Value cant be updated in redis");
            }

            return await GetBasketAsync(userId);
        }

        public async Task<Result> DeleteBasketAsync(string userId)
        {
            var spec = new TicketDeleteFromBasket(userId);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);


            //TODO
            foreach (var ticket in tickets)
            {
                ticket.StatusId = (int)StatusTypes.Free + 1;
                ticket.CustomerId = null;
                _unitOfWork.Repository<Ticket>().Update(ticket);
            }

            var updated = await _unitOfWork.Complete();
            if (updated < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be updated in db");
            }

            var removed = await _redisRepository.Remove(userId);
            if (!removed)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "No items in basket");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result<Dictionary<string, Basket>>> GetAllBaskets()
        {
            var basketList = await _redisRepository.GetList<Basket>();

            if (basketList == null)
            {
                return ResultReturnService.CreateErrorResult<Dictionary<string, Basket>>(ErrorStatusCode.NotFound, "No items in users baskets");
            }

            return new Result<Dictionary<string, Basket>>()
            {
                Value = basketList
            };
        }

        private async Task<Result<TimeSpan?>> GetExpiredTicketKeys(string userId)
        {
            var timeToLive = _redisRepository.TimeToExpire(userId);

            if (timeToLive == null)
            {
                var spec = new TicketDeleteFromBasket(userId);
                var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

                //TODO
                foreach (var ticket in tickets)
                {
                    ticket.StatusId = (int)StatusTypes.Free + 1;
                    ticket.CustomerId = null;
                    _unitOfWork.Repository<Ticket>().Update(ticket);
                }
                var updated = await _unitOfWork.Complete();

                if (updated < 0)
                {
                    return ResultReturnService.CreateErrorResult<TimeSpan?>(ErrorStatusCode.WrongAction, "Value cant be updated in db");
                }
            }

            return new Result<TimeSpan?>()
            {
                Value = timeToLive
            };
        }

        private decimal CalculateTotalPrice(List<int> ticketIds)
        {
            decimal totalPrice = 0;

            if (ticketIds != null && ticketIds.Count > 0)
            {
                //TODO
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
