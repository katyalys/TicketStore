using Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Specification
{
    public class GetTicketsSpec : BaseSpecification<OrderTicket>
    {
        public GetTicketsSpec()
        {
            AddInclude(x => x.Tickets);
        }
    }
}
