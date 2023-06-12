﻿using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.TicketsSpecifications
{
    public class FreeTicketsInfoFilterByPrice : BaseSpecification<Ticket>
    {
		public FreeTicketsInfoFilterByPrice(TicketSpecParam ticketSpecParam)
		: base(x => x.ConcertId == ticketSpecParam.ConcertId)
		{
			if (ticketSpecParam.MinPrice != null || ticketSpecParam.MaxPrice != null)
            {
				AddCriteria(x => (ticketSpecParam.MinPrice == null || x.Sector.Price >= ticketSpecParam.MinPrice) 
				&& (ticketSpecParam.MaxPrice == null || x.Sector.Price <= ticketSpecParam.MaxPrice));
            };

			AddOrderBy(x => x.Sector.Price);
			AddInclude(x => x.Concert);
			AddInclude(x => x.Sector);
			AddInclude(x => x.Status);
			AddCriteria(x => x.StatusId == (int)StatusTypes.Free);
			AddCriteria(x => x.Concert.IsDeleted == false);
		}
	}
}
