using Order.Domain.Enums;

namespace Order.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public int TicketBasketId { get; set; }
        public Status? TicketStatus { get; set; }
        public int? OrderTicketId { get; set; }
        public OrderTicket? Order { get; set; }
    }
}
