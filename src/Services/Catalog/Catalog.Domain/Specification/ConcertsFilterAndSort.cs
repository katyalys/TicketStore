using Catalog.Domain.Entities;

namespace Catalog.Domain.Specification
{
    public class ConcertsFilterAndSort: BaseSpecification<Concert>
    {

        public ConcertsFilterAndSort(ConcertsSpecParam specParam, bool isDescOrder) : base(c => !c.IsDeleted)
        {
            AddInclude(c => c.Place);

            if (specParam.City is not null || specParam.GenreName is not null)
            {
                AddCriteria(x =>
                    (specParam.City == null || x.Place.City == specParam.City) &&
                    (specParam.GenreName == null || x.GenreName == specParam.GenreName) 
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
