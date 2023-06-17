using Catalog.Application.Dtos.TicketDtos;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class TicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<IReadOnlyList<Ticket>> GetFreeTickets(TicketSpecParam ticketsSpec)
        {
            var spec = new FreeTicketsInfoFilterByPrice(ticketsSpec);
            var concerts = _unitOfWork.Repository<Ticket>().ListAsync(spec);

            return concerts;
        }

        public Task<IReadOnlyList<Ticket>> GetAllTickets(int concertId, bool isDescOredr)
        {
            var spec = new TicketsInfo(concertId, isDescOredr);
            var concerts = _unitOfWork.Repository<Ticket>().ListAsync(spec);

            return concerts;
        }

        public async Task AddTicketsAsync(List<Ticket> tickets)
        {
            foreach (var ticket in tickets)
            {
                ticket.Validate();
                await _unitOfWork.Repository<Ticket>().Add(ticket);
                await _unitOfWork.Complete();
            }
        }

        public async Task DeleteTickets(List<int> ticketsIds, int concertId)
        {
            var spec = new TicketDelete(ticketsIds, concertId);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

            foreach (var ticket in tickets)
            {
                _unitOfWork.Repository<Ticket>().Delete(ticket);
            }
            await _unitOfWork.Complete();
        }
    }
}
