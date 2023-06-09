﻿using Shared.EventBus.Messages.Enums;

namespace Shared.EventBus.Messages.Events
{
    public class GetTicketStatusEvent : IntegrationBaseEvent
    {
        public List<int> TicketBasketId { get; set; }
        public MessageStatus TicketStatus { get; set; }
        public string? UserId { get; set; }
    }
}
