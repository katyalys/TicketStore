using Catalog.Application.Dtos.TicketDtos;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Specification.TicketsSpecifications;

namespace Catalog.Application.Interfaces
{
    public interface ITicketService
    {
        Task<Result<IReadOnlyList<TicketDto>?>> GetFreeTicketsAsync(TicketSpecParam ticketsSpec);
        Task<Result<IReadOnlyList<TicketDto>?>> GetAllTicketsAsync(int concertId, bool isDescOredr);
        Task<Result> AddTicketsAsync(List<TicketAddDto> ticketsDto);
        Task<Result> DeleteTicketsAsync(List<int> ticketsIds, int concertId);
    }
}
