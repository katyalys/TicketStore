using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.ConcertSpecifications;
using Catalog.Domain.Specification.TicketsSpecifications;
using Catalog.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class BackgroundJobsService : IBackgroundJobsService
    {
        private readonly IRedisRepository _redisRepository;
        private readonly CatalogContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public BackgroundJobsService(IRedisRepository redisRepository, CatalogContext context, IUnitOfWork unitOfWork)
        {
            _redisRepository = redisRepository;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> AddBasketUpdate(int ticketId, string userId)
        {
            var spec = new TicketAddToBasket(ticketId);
            var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);

            if (ticket == null)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, " Сant add ticket");
            }

            ticket.StatusId = (int)StatusTypes.Book + 1;
            ticket.CustomerId = userId;
            _unitOfWork.Repository<Ticket>().Update(ticket);
            var updated = await _unitOfWork.Complete();

            if (updated < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be updated in db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task UpdateDeletedConcerts()
        {
            DateTime currentDate = DateTime.Now;
            var concertsWithDate = new ConcertsWithDate(currentDate);
            var concertsToUpdate = await _unitOfWork.Repository<Concert>().ListAsync(concertsWithDate);

            foreach (var concert in concertsToUpdate)
            {
                concert.IsDeleted = true;
                concert.Tickets.ForEach(ticket => ticket.IsDeleted = true);
            }

            var updated = await _unitOfWork.Complete();
        }


    }
}
