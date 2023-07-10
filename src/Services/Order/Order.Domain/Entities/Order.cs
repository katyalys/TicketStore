using Order.Domain.Enums;

namespace Order.Domain.Entities
{
    public class OrderTicket : BaseEntity
    {
        public string CustomerId { get; set; }
        public Status OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public List<Ticket> Tickets { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
