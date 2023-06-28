using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands
{
    public class CheckoutOrderCommand
    {
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<int> TicketIds { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
