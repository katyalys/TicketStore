using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Specification.TicketsSpecifications
{
    public class TicketsWithStatus : BaseSpecification<Ticket>
    {
        public TicketsWithStatus(int concertId)
        {
            AddInclude(x => x.Status);
            AddCriteria(x => x.ConcertId == concertId && x.StatusId == (int)StatusTypes.Bought + 1);
        }
    }
}
