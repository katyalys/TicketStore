using Order.Domain.Entities;

namespace Order.Domain.Specification.TicketSpecifications
{
    public class TicketsByOrderSpec : BaseSpecification<Ticket>
    {
        public TicketsByOrderSpec(int orderId)
        {
            AddCriteria(ticket => ticket.OrderTicketId == orderId);
        }
    }
}
