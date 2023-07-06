using MediatR;
using Order.Application.Dtos;
using Order.Domain.ErrorModels;

namespace Order.Application.Features.Orders.Queries.TicketDetailedInfo
{
    public class TicketsDetailedQuery : IRequest<Result<List<TicketDetailInfoDto>>>
    {
        public string? CustomerId { get; set; }
        public int OrderId { get; set; }
    }
}
