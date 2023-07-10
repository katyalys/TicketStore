using MediatR;
using Order.Domain.ErrorModels;

namespace Order.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommand : IRequest<Result<int>>
    {
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<int> TicketIds { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
