﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<bool>
    {
        [JsonIgnore]
        public string? CustomerId { get; set; }
        public int OrderId { get; set; }
    }
}
