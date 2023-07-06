using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.ConcertSpecifications;
using Catalog.Domain.Specification.TicketsSpecifications;

namespace Catalog.Infrastructure.Services
{
    public class BackgroundJobsService : IBackgroundJobsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BackgroundJobsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddBasketUpdate(Ticket ticket, string userId)
        {
            ticket.StatusId = (int)StatusTypes.Book + 1;
            ticket.CustomerId = userId;
            _unitOfWork.Repository<Ticket>().Update(ticket);
            await _unitOfWork.Complete();
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

            await _unitOfWork.Complete();
        }

        public async Task DeleteBasket(string userId)
        {
            var spec = new TicketDeleteFromBasket(userId);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

            //TODO
            foreach (var ticket in tickets)
            {
                if (ticket.StatusId != (int)StatusTypes.Bought + 1)
                {
                    ticket.StatusId = (int)StatusTypes.Free + 1;
                    ticket.CustomerId = null;
                    _unitOfWork.Repository<Ticket>().Update(ticket);
                }
            }

            await _unitOfWork.Complete();
        }

    }
}
