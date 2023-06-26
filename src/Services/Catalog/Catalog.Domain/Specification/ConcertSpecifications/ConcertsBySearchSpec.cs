using Catalog.Domain.Entities;

namespace Catalog.Domain.Specification.ConcertSpecifications
{
    public class ConcertsBySearchSpec : BaseSpecification<Concert>
    {
        public ConcertsBySearchSpec(string searchTerm) :
                                 base(c => (c.Name.Contains(searchTerm) || c.Perfomer.Contains(searchTerm)) && !c.IsDeleted)
        {
            AddInclude(x => x.Place);
        }
    }
}
