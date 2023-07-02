using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Dtos
{
    public class TicketDetailInfoDto
    {
        public string Concert { get; set; }
        public string Performer { get; set; }
        public string Genre { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public decimal Price { get; set; }
        public string Sector { get; set; }
        public int Row { get; set; }
        public int Seat { get; set; }
    }
}
