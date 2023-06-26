using Catalog.Application.Dtos.TicketDtos;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
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
        Task<Result<IReadOnlyList<TicketDto>?>> GetFreeTickets(TicketSpecParam ticketsSpec);
        Task<Result<IReadOnlyList<TicketDto>?>> GetAllTickets(int concertId, bool isDescOredr);
        Task<Result> AddTicketsAsync(List<TicketAddDto> ticketsDto);
        Task<Result> DeleteTickets(List<int> ticketsIds, int concertId);
    }
}
