using MediatR;
using Order.Application.Dtos;
using Order.Domain.ErrorModels;
using System.Text.Json.Serialization;

namespace Order.Application.Features.Orders.Queries.TicketDetailedInfo
{
    public class TicketsDetailedQuery : IRequest<Result<List<TicketDetailInfoDto>>>
    {
        [JsonIgnore]
        public string? CustomerId { get; set; }
        public int OrderId { get; set; }
    }
}
