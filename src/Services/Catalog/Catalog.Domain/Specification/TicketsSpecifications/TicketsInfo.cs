using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.TicketsSpecifications
{
	public class TicketsInfo : BaseSpecification<Ticket>
	{
		public TicketsInfo(int concertId, bool isDescOrder)
		: base(x => x.ConcertId == concertId)
		{
			AddInclude(x => x.Concert);
			AddInclude(x => x.Sector);
			AddInclude(x => x.Status);
			AddCriteria(x => x.StatusId == (int)StatusTypes.Free);

			if (isDescOrder)
			{
				AddOrderByDescending(x => x.Sector.Price);
			}
			else
			{
				AddOrderBy(x => x.Sector.Price);
			}
		}

		public TicketsInfo(int ticketId)
			: base(x => x.Id == ticketId)
		{
			AddInclude(x => x.Concert);
			AddInclude(x => x.Concert.Place);
			AddInclude(x => x.Sector);
			AddInclude(x => x.Status);
		}
	}
}
