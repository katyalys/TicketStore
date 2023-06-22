using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos.TicketDtos
{
    public class TicketDto
    {
        public ConcertsShortViewDto Concert { get; set; }
        public decimal Price { get; set; }
        public string SectorName { get; set; }
        public int Row { get; set; }
        public int Seat { get; set; }
    }
}
