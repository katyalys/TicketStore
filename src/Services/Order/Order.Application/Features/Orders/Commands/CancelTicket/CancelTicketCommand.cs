using MediatR;
using Order.Domain.ErrorModels;

namespace Order.Application.Features.Orders.Commands.CancelTicket
{
    public class CancelTicketCommand : IRequest<Result>
    {
        public string? CustomerId { get; set; }
        public int OrderId { get; set; }
        public int TicketId { get; set; }
    }
}
