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
        public int RowSeatNumber { get; set; } //kol-vo mest v ryady
        public decimal Price { get; set; }
        public string? PictureLink { get; set; }

        public int? PlaceId { get; set; }
        public Place? Place { get; set; }

        public List<Ticket>? Tickets { get; set; } = new();

        public decimal SetPriceBasedOnName(string name)
        {

            switch (name)
            {
                case nameof(SectorName.DanceFloor):
                    return Price = 25.99m;
                    break;
                case nameof(SectorName.A):
                    return Price = 50.99m;
                    break;
                case nameof(SectorName.B):
                    return Price = 40.99m;
                    break;
                case nameof(SectorName.C):
                    return Price = 30.99m;
                    break;
                case nameof(SectorName.D):
                    return Price = 20.99m;
                    break;
                case nameof(SectorName.F):
                    return Price = 10.99m;
                    break;
                default:
                    return Price = 0.00m;
                    break;
            }
        }
    }

    public enum SectorName
    {
        DanceFloor,
        A,
        B,
        C,
        D,
        E,
        F,
    }
}
