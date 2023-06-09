using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Status
    {
        public int Id { get; set; }
        public StatusTypes StatusType { get; set; }
        public List<Ticket>? Tickets { get; set; } = new();
    }

    public enum StatusTypes { 
        Bought,
        Free,
    }

}
