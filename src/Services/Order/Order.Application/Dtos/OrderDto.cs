using Order.Domain.Enums;

namespace Order.Application.Dtos
{
    public class OrderDto
    {
        public List<TicketDetailInfoDto> TicketDetails { get; set; }
        public Status OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerId { get; set; }
    }
}
