using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification
{
    public class ConcertWithValidTickets : BaseSpecification<Concert>
    {
        public ConcertWithValidTickets(int concertId)
            : base(c => c.Id == concertId)
        {
            AddInclude(c => c.Tickets);
            AddCriteria(c => c.Tickets != null && c.Tickets.Any(t => !t.IsDeleted) && c.Id == concertId);
        }
    }

}
