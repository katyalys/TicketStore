using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.TicketsSpecifications
{
    public class TicketDelete : BaseSpecification<Ticket>
	{
		public TicketDelete(List<int> ticketsIds, int concertId)
		{
			AddInclude(x => x.Status);
			AddCriteria(x => ticketsIds.Contains(x.Id) && x.Status != null && x.StatusId == (int)StatusTypes.Free + 1 
							&& x.ConcertId == concertId);
		}
	}
}
