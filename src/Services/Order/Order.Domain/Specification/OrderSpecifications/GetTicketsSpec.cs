using Order.Domain.Entities;

namespace Order.Domain.Specification.OrderSpecifications
{
    public class GetTicketsSpec : BaseSpecification<OrderTicket>
    {
        public GetTicketsSpec()
        {
            AddInclude(x => x.Tickets);
        }
    }
}
