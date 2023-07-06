using Order.Domain.Entities;

namespace Order.Domain.Specification.OrderSpecifications
{
    public class OrderByCustomerSpec : BaseSpecification<OrderTicket>
    {
        public OrderByCustomerSpec(string customerId)
        {
            AddInclude(x => x.Tickets);
            AddCriteria(x => x.CustomerId == customerId);
        }
    }
}
