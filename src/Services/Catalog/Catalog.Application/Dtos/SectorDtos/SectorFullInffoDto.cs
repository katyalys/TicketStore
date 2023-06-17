using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos.SectorDtos
{
    public class SectorFullInffoDto
    {

        public string Name { get; set; }
        public int RowNumber { get; set; }
        public int RowSeatNumber { get; set; } //kol-vo mest v ryady
        public decimal Price { get; set; }
        public string? PictureLink { get; set; }
        public int PlaceId { get; set; }
    }
}
