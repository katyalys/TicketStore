using MediatR;
using Order.Domain.ErrorModels;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<Result>
    {
        public string? CustomerId { get; set; }
        public int OrderId { get; set; }
    }
}
