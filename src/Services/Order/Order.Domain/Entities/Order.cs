using Order.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public OrderStatus Status { get; set; }  
        public DateTime OrderDate { get; set; }
        public List<int> TicketIds { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
