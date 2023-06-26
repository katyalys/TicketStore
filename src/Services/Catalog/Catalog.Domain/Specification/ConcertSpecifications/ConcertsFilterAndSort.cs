using Catalog.Domain.Entities;

namespace Catalog.Domain.Specification.ConcertSpecifications
{
    public class ConcertsFilterAndSort : BaseSpecification<Concert>
    {
        public ConcertsFilterAndSort(ConcertsSpecParam specParam, bool isDescOrder)
        {
            AddInclude(c => c.Place);
            AddCriteria(c => !c.IsDeleted);

            if (specParam.City is not null || specParam.GenreName is not null)
            {
                AddCriteria(x =>
                    (specParam.City == null || x.Place.City == specParam.City) &&
                    (specParam.GenreName == null || x.GenreName == specParam.GenreName) &&
                    (x.IsDeleted == false)
                );
            }

            if (isDescOrder)
            {
                AddOrderByDescending(c => c.Date);
            }
            else
            {
                AddOrderBy(c => c.Date);
            }
        }
    }
}
