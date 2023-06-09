using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Concert
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Perfomer { get; set; }

        // фк на жанр
        public string GenreName { get; set; }

        //фк на концертный зал
      //  public string? PlaceId { get; set; }
        public Place? Place { get; set; }
        public bool IsDeleted { get; set; }
        public List<Ticket>? Tickets { get; set; } = new();
    }
}
