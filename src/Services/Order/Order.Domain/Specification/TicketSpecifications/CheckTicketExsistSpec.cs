using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Domain.Specification.TicketSpecifications
{
    public class CheckTicketExsistSpec : BaseSpecification<Ticket>
    {
        public CheckTicketExsistSpec(int ticketId, int orderId, string customerId)
        {
            AddInclude(order => order.Order);
            AddCriteria(ticket =>
                ticket.Id == ticketId &&
                ticket.TicketStatus == Status.Paid &&
                ticket.OrderTicketId == orderId &&
                ticket.Order.CustomerId == customerId);
        }
    }
}
