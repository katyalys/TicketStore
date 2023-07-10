using MediatR;
using Order.Application.Dtos;
using Order.Domain.ErrorModels;

namespace Order.Application.Features.Orders.Queries.OrderHistory
{
    public class OrderHistoryQuery : IRequest<Result<List<FullOrderDto>>>
    {
        public string CustomerId { get; set; }
    }
}
