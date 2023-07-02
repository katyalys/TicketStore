using MediatR;
using Order.Application.Dtos;
using Order.Domain.ErrorModels;

namespace Order.Application.Features.Orders.Queries.AllOrders
{
    public class AllOrdersQuery : IRequest<Result<List<OrderDto>>>
    {
    }
}
