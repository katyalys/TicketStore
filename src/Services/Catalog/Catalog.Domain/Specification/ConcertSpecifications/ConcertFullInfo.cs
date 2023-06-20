using Catalog.Domain.Entities;

namespace Catalog.Domain.Specification.ConcertSpecifications
{
    public class ConcertFullInfo : BaseSpecification<Concert>
    {
		public ConcertFullInfo(int id)
		: base(x => x.Id == id)
		{
			AddInclude(x => x.Place);
		}

		public ConcertFullInfo()
		: base()
		{
			AddInclude(x => x.Place);
		}
	}
}
