using MediatR;
using Order.Application.Dtos;
using OrderClientGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries.TicketDetailedInfo
{
    public class TicketsDetailedQuery : IRequest<List<TicketDetailInfo>>
    {
        //fluent validation
        [JsonIgnore]
        public string? CustomerId { get; set; }
        public int OrderId { get; set; }
        //public int TicketId { get; set; }
    }
}
