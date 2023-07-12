using Catalog.Domain.Entities;

namespace Catalog.Domain.Specification.ConcertSpecifications
{
    public class ConcertsWithDate : BaseSpecification<Concert>
    {
        public ConcertsWithDate(DateTime currentDate)
        {
            AddCriteria(c => c.Date < currentDate && c.IsDeleted == false);
            AddInclude(c => c.Tickets);
        }
    }
}
