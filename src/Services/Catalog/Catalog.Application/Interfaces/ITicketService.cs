using Catalog.Application.Dtos.TicketDtos;
using Catalog.Domain.Entities;
using Catalog.Domain.Specification.TicketsSpecifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface ITicketService
    {
        Task<IReadOnlyList<TicketDto>?> GetFreeTickets(TicketSpecParam ticketsSpec);
        Task<IReadOnlyList<TicketDto>?> GetAllTickets(int concertId, bool isDescOredr);
        Task AddTicketsAsync(List<TicketAddDto> ticketsDto);
        Task DeleteTickets(List<int> ticketsIds, int concertId);
    }
}
