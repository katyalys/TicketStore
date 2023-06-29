using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos
{
    public class TicketOrderInfoDto
    {
        public decimal Price { get; set; }
        public string SectorName { get; set; }
        public int Row { get; set; }
        public int Seat { get; set; }
        public string Place { get; set; }
        public string Concert { get; set; }
        public DateTime Date { get; set; }
    }
}
