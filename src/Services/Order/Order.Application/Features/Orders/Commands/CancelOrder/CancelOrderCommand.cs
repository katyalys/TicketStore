using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<bool>
    {
        public int OrderId { get; set; }
    }
}
