using AutoMapper;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Interfaces;
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
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TicketService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IReadOnlyList<TicketDto>?> GetFreeTickets(TicketSpecParam ticketsSpec)
        {
            var spec = new FreeTicketsInfoFilterByPrice(ticketsSpec);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);
            var ticketDto = _mapper.Map<IReadOnlyList<TicketDto>>(tickets);

            return ticketDto;
        }

        public async Task<IReadOnlyList<TicketDto>?> GetAllTickets(int concertId, bool isDescOredr)
        {
            var spec = new TicketsInfo(concertId, isDescOredr);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);
            var ticketDto = _mapper.Map<IReadOnlyList<TicketDto>>(tickets);

            return ticketDto;
        }

        public async Task AddTicketsAsync(List<TicketAddDto> ticketsDto)
        {
            var tickets = _mapper.Map<IReadOnlyList<Ticket>>(ticketsDto);
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
