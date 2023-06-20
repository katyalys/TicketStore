using Catalog.Application.Dtos.TicketDtos;
using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos.BasketDtos
{
    public class BasketDto
    {
        public List<TicketDto> Tickets { get; set; }
        public decimal TotalPrice { get; set; }
        public TimeSpan TimeToBuy { get; set; }
    }
}
