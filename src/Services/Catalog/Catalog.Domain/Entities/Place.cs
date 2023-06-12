using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Place: BaseEntity
    {
        public string City { get; set; }
        public string Street { get; set; }
        public int PlaceNumber { get; set; }
     //   public bool IsDeleted { get; set; }
        public List<Concert>? Concerts { get; set; } = new();
        public List<Sector>? Sectors { get; set; } = new();
    }
}
