using Order.Domain.Entities;
using Order.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
				//Id = 1,
				CustomerId = "12345",
				OrderStatus = Status.Paid,
				OrderDate = DateTime.Now,
				TotalPrice = 150.00m
			},
			new OrderTicket
			{
				//Id = 2,
				CustomerId = "54321",
				OrderStatus = Status.Canceled,
				OrderDate = DateTime.Now.AddDays(-1),
				TotalPrice = 100.00m
			});
			await context.SaveChangesAsync();

			await context.AddRangeAsync(
			new Ticket
			{
				//Id = 1,
				TicketBasketId = 1,
				Sector = "A",
				Row = 1,
				Seat = 10,
				Place = "Venue A",
				Concert = "Concert X",
				TicketStatus = Status.Paid,
				Date = DateTime.Now.AddDays(7),
				OrderTicketId = 1
			},
			new Ticket
			{
				//Id = 2,
				TicketBasketId = 2,
				Sector = "B",
				Row = 2,
				Seat = 5,
				Place = "Venue A",
				Concert = "Concert X",
				TicketStatus = Status.Canceled,
				Date = DateTime.Now.AddDays(7),
				OrderTicketId = 1
			},
			new Ticket
            {
               // Id = 3,
				TicketBasketId = 3,
				Sector = "C",
                Row = 3,
                Seat = 20,
                Place = "Venue A",
                Concert = "Concert X",
                TicketStatus = Status.Paid,
                Date = DateTime.Now.AddDays(7),
                OrderTicketId = 1
            },
            new Ticket
			{
				//Id = 4,
				TicketBasketId = 4,
				Sector = "D",
				Row = 1,
				Seat = 15,
				Place = "Venue B",
				Concert = "Concert Y",
				TicketStatus = Status.Paid,
				Date = DateTime.Now.AddDays(14),
				OrderTicketId = 2,
			},
			new Ticket
			{
				//Id = 5,
				TicketBasketId = 5,
				Sector = "E",
				Row = 2,
				Seat = 8,
				Place = "Venue B",
				Concert = "Concert Y",
				TicketStatus = Status.Canceled,
				Date = DateTime.Now.AddDays(14),
				OrderTicketId = 2
			});
			await context.SaveChangesAsync();
		}
	}
}
