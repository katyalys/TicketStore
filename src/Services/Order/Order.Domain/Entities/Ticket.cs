using Order.Domain.Enums;

namespace Order.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public int TicketBasketId { get; set; }
        public string? Sector { get; set; }   // remove
        public int? Row { get; set; }   // remove
        public int? Seat { get; set; }  // remove
        public string? Place { get; set; }  // remove
        public string? Concert { get; set; }    // remove
        public Status? TicketStatus { get; set; }  
        public DateTime? Date { get; set; }     // remove
        public int? OrderTicketId { get; set; }     
        public OrderTicket? Order { get; set; }  
    }
}
