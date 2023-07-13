using Order.Domain.Entities;

namespace Order.Domain.Specification.TicketSpecifications
{
    public class TicketsByOrdersListSpec : BaseSpecification<Ticket>
    {
        public TicketsByOrdersListSpec(List<int> orderIds)
        {
            AddCriteria(ticket => orderIds.Contains(ticket.OrderTicketId));
        }
    }
}
