using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Basket
    {

        public List<int> TicketIds { get; set; }
        public decimal TotalPrice { get; set; }
        public TimeSpan TimeToBuy { get; set; }
    }
}
