using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.TicketsSpecifications
{
    public class TicketAddToBasket : BaseSpecification<Ticket>
	{
		public TicketAddToBasket(int ticketId) :  base(x => x.Id == ticketId)
		{
			AddCriteria(x => x.Id == ticketId && x.CustomerId == null && x.IsDeleted == false);
		}
	}
}
