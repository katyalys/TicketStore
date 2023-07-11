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
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRedisRepository _redisRepository;
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BasketService> _logger;

        public BasketService(IRedisRepository redisRepository, IMapper mapper, IUnitOfWork unitOfWork, 
            CatalogContext context, ILogger<BasketService> logger)
        {
            _mapper = mapper;
            _redisRepository = redisRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _logger = logger;
        }

        public async Task<Result<BasketDto>> AddBasketTicketAsync(int ticketId, string userId)
        {
            var spec = new TicketAddToBasket(ticketId);
            var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpecAsync(spec);

            if (ticket == null)
            {
                _logger.LogError("Ticket not found for ID {TicketId}", ticketId);

                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, " Сant add ticket");
            }

            HangfireUpdateBasket.UpdateBasket(ticket, userId);
            var basket = await _redisRepository.GetAsync<Basket>(userId);

            if (basket == null)
            {
                basket = new Basket();
                basket.TicketIds = new List<int>();
            }

            basket.TicketIds.Add(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);
            var result = await _redisRepository.AddAsync(userId, basket, TimeSpan.FromMinutes(20));

            if (result == false)
            {
                _logger.LogError("Failed to add value to Redis");

                return ResultReturnService.CreateErrorResult<BasketDto>
                    (ErrorStatusCode.WrongAction, "Value cant be added to redis");
            }

            _logger.LogInformation("Ticket successfully added to the basket");

            return await GetBasketAsync(userId);
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string userId)
        {

            var basket = await _redisRepository.GetAsync<Basket>(userId);

            if (basket == null)
            {
                _logger.LogWarning("No items found in the basket for user ID {UserId}", userId);

                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, "No itmes in basket");
            }

            var basketDto = _mapper.Map<BasketDto>(basket);
            var expire = _redisRepository.TimeToExpire(userId);
            basketDto.TimeToBuy = (TimeSpan)expire.Value;
            basketDto.Tickets = new List<TicketDto>();

            //TODO
            foreach (var ticketId in basket.TicketIds)
            {
                var spec = new TicketsInfo(ticketId);
                var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpecAsync(spec);

                if (ticket == null)
                {
                    _logger.LogError("Failed to get ticket information for ticket ID {TicketId}", ticketId);

                    return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, "Ticket not found");
                }

                var ticketDto = _mapper.Map<TicketDto>(ticket);
                basketDto.Tickets.Add(ticketDto);
            }

            _logger.LogInformation("Basket with user id {UserId} was successfully recieved", userId);

            return new Result<BasketDto>()
            {
                Value = basketDto
            };
        }

        public async Task<Result<BasketDto>> DeleteFromBasketTicketAsync(int ticketId, string userId)
        {
            var basket = await _redisRepository.GetAsync<Basket>(userId);

            if (basket == null)
            {
                _logger.LogWarning("No items found in the basket for user ID {UserId}", userId);

                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, "No items in basket");
            }

            basket.TicketIds.Remove(ticketId);
            basket.TotalPrice = CalculateTotalPrice(basket.TicketIds);

            var spec = new TicketDeleteFromBasket(userId, ticketId);
            var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpecAsync(spec);

            if (ticket == null)
            {
                _logger.LogError("Failed to delete ticket from the basket. Ticket not found for ID {TicketId}", ticketId);

                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.NotFound, "Ticket not found");
            }

            ticket.StatusId = (int)StatusTypes.Free + 1;
            ticket.CustomerId = null;
            _unitOfWork.Repository<Ticket>().Update(ticket);
            var updated = await _unitOfWork.CompleteAsync();

            if (updated < 0)
            {
                _logger.LogError("Failed to update ticket in the database. Ticket ID {TicketId}", ticketId);

                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.WrongAction, "Value cant be updated in db");
            }

            var updatedBasket = await _redisRepository.UpdateAsync<Basket>(userId, basket, TimeSpan.FromMinutes(20));

            if (updatedBasket == false)
            {
                _logger.LogError("Failed to update basket in Redis. User ID {UserId}", userId);

                return ResultReturnService.CreateErrorResult<BasketDto>(ErrorStatusCode.WrongAction, "Value cant be updated in redis");
            }

            _logger.LogInformation("Ticket successfully deleted from the basket");

            return await GetBasketAsync(userId);
        }

        public async Task<Result> DeleteBasketAsync(string userId)
        {
            var removed = await _redisRepository.RemoveAsync(userId);

            if (!removed)
            {
                _logger.LogError("Failed to delete basket. No items found for user ID {UserId}", userId);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "No items in basket");
            }

            _logger.LogInformation("Basket successfully deleted");

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result<Dictionary<string, Basket>>> GetAllBasketsAsync()
        {
            var basketList = await _redisRepository.GetListAsync<Basket>();

            if (basketList == null)
            {
                _logger.LogWarning("No items found in any user's basket");

                return ResultReturnService.CreateErrorResult<Dictionary<string, Basket>>(ErrorStatusCode.NotFound, "No items in users baskets");
            }

            _logger.LogInformation("All baskets were successfully retrieved");

            return new Result<Dictionary<string, Basket>>()
            {
                Value = basketList
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
