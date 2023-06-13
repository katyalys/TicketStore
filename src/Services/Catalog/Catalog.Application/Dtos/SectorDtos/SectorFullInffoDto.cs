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

      //  private decimal price;
        public decimal Price { get; set; }
        //{
        //    get { return price; }
        //    set { price = TicketPriceCalculator.CalculatePrice(value); }
        //}

        public string? PictureLink { get; set; }

        public int PlaceId { get; set; }
    }
}
