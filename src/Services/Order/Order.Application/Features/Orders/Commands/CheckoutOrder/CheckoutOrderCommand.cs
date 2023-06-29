using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommand : IRequest<int>
    {
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<int> TicketIds { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
