using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Domain.Specification.TicketSpecifications
{
    public class TicketSpec : BaseSpecification<Ticket>
    {
        public TicketSpec(int ticketId, int orderId)
        {
            AddCriteria(ticket =>
                ticket.TicketBasketId == ticketId &&
                ticket.TicketStatus != Status.Canceled &&
                ticket.OrderTicketId == orderId);
        }
    }
}
