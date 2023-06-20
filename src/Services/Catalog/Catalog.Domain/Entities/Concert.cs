using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Concert: BaseEntity
    {
        public DateTime Date { get; set; }
        public string? PosterLink { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Perfomer { get; set; }
        public string GenreName { get; set; }
        public int? PlaceId { get; set; }
        public Place? Place { get; set; }
        public List<Ticket>? Tickets { get; set; } = new();
    }
}
