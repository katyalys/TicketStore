using Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Sector: BaseEntity
    {
        public SectorName Name { get; set; }
        public int RowNumber { get; set; }
        public int RowSeatNumber { get; set; }

        public decimal Price { get; set; }
        public string? PictureLink { get; set; }

        public int? PlaceId { get; set; }
        public Place? Place { get; set; }

        public List<Ticket>? Tickets { get; set; } = new();
    }

    //public enum SectorName
    //{
    //    DanceFloor,
    //    A,
    //    B,
    //    C,
    //    D,
    //    E,
    //    F,
    //}
}
