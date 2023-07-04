using Shared.EventBus.Messages.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.EventBus.Messages.Events
{
    public class GetTicketStatusEvent : IntegrationBaseEvent
    {
        public List<int> TicketBasketId { get; set; }
        public Status TicketStatus { get; set; }
    }
}
