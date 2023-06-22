using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.TicketsSpecifications
{
    public class TicketDeleteFromBasket : BaseSpecification<Ticket>
    {
        public TicketDeleteFromBasket(string userId)
        {
            AddCriteria(x => x.CustomerId == userId);
            AddInclude(x => x.Status);
        }

        public TicketDeleteFromBasket(string userId, int ticketId)
        {
            AddCriteria(x => x.CustomerId == userId && x.Id == ticketId);
            AddInclude(x => x.Status);
        }
    }
}
