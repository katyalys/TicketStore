using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.ConcertSpecifications;
using Catalog.Domain.Specification.TicketsSpecifications;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services
{
    public class BackgroundJobsService : IBackgroundJobsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BackgroundJobsService> _logger;

        public BackgroundJobsService(IUnitOfWork unitOfWork, ILogger<BackgroundJobsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task AddBasketUpdateAsync(Ticket ticket, string userId)
        {
            ticket.StatusId = (int)StatusTypes.Book + 1;
            ticket.CustomerId = userId;
            _unitOfWork.Repository<Ticket>().Update(ticket);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Basket update successfully added. Ticket ID: {TicketId}, User ID: {UserId}", ticket.Id, userId);
        }

        public async Task UpdateDeletedConcertsAsync()
        {
            DateTime currentDate = DateTime.Now;
            var concertsWithDate = new ConcertsWithDate(currentDate);
            var concertsToUpdate = await _unitOfWork.Repository<Concert>().ListAsync(concertsWithDate);

            foreach (var concert in concertsToUpdate)
            {
                concert.IsDeleted = true;
                concert.Tickets.ForEach(ticket => ticket.IsDeleted = true);
            }

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Deleted concerts successfully updated.");
        }

        public async Task DeleteBasketAsync(string userId)
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

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Basket successfully deleted. User ID: {UserId}", userId);
        }

    }
}
