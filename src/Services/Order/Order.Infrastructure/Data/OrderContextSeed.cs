using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext context)
        {
            if (context.Orders.Any())
                return;

            await context.AddRangeAsync(
            new OrderTicket
            {
                CustomerId = "12345",
                OrderStatus = Status.Paid,
                OrderDate = DateTime.Now,
                TotalPrice = 150.00m
            },
            new OrderTicket
            {
                CustomerId = "54321",
                OrderStatus = Status.Canceled,
                OrderDate = DateTime.Now.AddDays(-1),
                TotalPrice = 100.00m
            });
            await context.SaveChangesAsync();

            await context.AddRangeAsync(
            new Ticket
            {
                TicketBasketId = 1,
                TicketStatus = Status.Canceled,
                OrderTicketId = 1
            },
            new Ticket
            {
                TicketBasketId = 2,
                TicketStatus = Status.Paid,
                OrderTicketId = 1
            },
            new Ticket
            {
                TicketBasketId = 3,
                TicketStatus = Status.Paid,
                OrderTicketId = 1
            },
            new Ticket
            {
                TicketBasketId = 4,
                TicketStatus = Status.Canceled,
                OrderTicketId = 2,
            },
            new Ticket
            {
                TicketBasketId = 5,
                TicketStatus = Status.Paid,
                OrderTicketId = 2
            });
            await context.SaveChangesAsync();
        }
    }
}
