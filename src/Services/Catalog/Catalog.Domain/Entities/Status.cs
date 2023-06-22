using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Status: BaseEntity
    {
        public string Name { get; set; }
        public List<Ticket>? Tickets { get; set; } = new();
    }
}
