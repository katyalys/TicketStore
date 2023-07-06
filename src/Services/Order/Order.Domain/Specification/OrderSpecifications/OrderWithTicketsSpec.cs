using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Domain.Specification.OrderSpecifications
{
    public class OrderWithTicketsSpec : BaseSpecification<OrderTicket>
    {
        public OrderWithTicketsSpec(int orderId, string customerId)
        {
            AddInclude(o => o.Tickets);
            AddCriteria(o => o.Id == orderId &&
                o.CustomerId == customerId &&
                o.OrderStatus != Status.Canceled);
        }
    }
}
