using MediatR;
using Order.Domain.ErrorModels;
using System.Text.Json.Serialization;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<Result>
    {
        [JsonIgnore]
        public string? CustomerId { get; set; }
        public int OrderId { get; set; }
    }
}
