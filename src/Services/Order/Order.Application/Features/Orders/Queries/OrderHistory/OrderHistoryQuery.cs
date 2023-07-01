using MediatR;
using Order.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries.OrderHistory
{
    public class OrderHistoryQuery : IRequest<List<FullOrderDto>>
    {
        public string CustomerId { get; set; }
    }
}
