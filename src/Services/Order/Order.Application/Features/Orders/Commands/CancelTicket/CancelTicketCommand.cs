using MediatR;
using Order.Domain.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CancelTicket
{
    public class CancelTicketCommand : IRequest<Result>
    {
        public string? CustomerId { get; set; }
        public int OrderId { get; set; }
        public int TicketId { get; set; }
    }
}
