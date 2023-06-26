using Catalog.Domain;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data
{
    public class CatalogContextSeed
    {
        public static async Task SeedAsync(CatalogContext context)
        {
            if (context.Concerts.Any())
                return;

            await context.AddRangeAsync(new Status
			{
				Name = "Bought",
			},
			new Status
			{
				Name = "Free",
			},
            new Status
            {
                Name = "Book",
            });
			await context.SaveChangesAsync();

			await context.AddRangeAsync(new Place
			{
				City = "Minsk",
				Street = "Yakuba Kolasa",
				PlaceNumber = 28,
			},
			new Place
			{
				City = "Gomel",
				Street = "Novopolesskaya",
				PlaceNumber = 36,
			},
			new Place
			{
				City = "Minsk",
				Street = "Piatra Mscislavca",
				PlaceNumber = 9,
			});
			await context.SaveChangesAsync();

			var place1 = context.Places.Where(x => x.Street == "Novopolesskaya").Select(s => s).FirstOrDefault();
			var place2 = context.Places.Where(x => x.Street == "Piatra Mscislavca").Select(s => s).FirstOrDefault();

			await context.AddRangeAsync(new Concert
			{
				Date = new DateTime(2023, 12, 25),
				Description = "But I must explain to you how all this mistaken idea of " +
				"denouncing pleasure and praising pain was born and I will give you " +
				"a complete account of the system, and expound the actual teachings of" +
				" the great explorer of the truth, the master-builder of human happiness." +
				" No one rejects, dislikes, or avoids pleasure itself, because it is pleasure," +
				" but because those who do not know how to pursue pleasure rationally encounter ",
				GenreName = "pop",
				Name = "Evolve Tour",
				Perfomer = "Imagine Dragons",
				Place = place1,
			},
			new Concert
			{
				Date = new DateTime(2023, 11, 15),
				Description = "But I must explain to you how all this mistaken idea of " +
				"denouncing pleasure and praising pain was born and I will give you " +
				"a complete account of the system, and expound the actual teachings of" +
				" the great explorer of the truth, the master-builder of human happiness." +
				" No one rejects, dislikes, or avoids pleasure itself, because it is pleasure," +
				" but because those who do not know how to pursue pleasure rationally encounter ",
				GenreName = "rock",
				Name = "POST HUMAN",
				Perfomer = "Bring me the horizon",
				Place = place2,
            },
			new Concert
			{
				Date = new DateTime(2024, 8, 4),
				Description = "But I must explain to you how all this mistaken idea of " +
				"denouncing pleasure and praising pain was born and I will give you " +
				"a complete account of the system, and expound the actual teachings of" +
				" the great explorer of the truth, the master-builder of human happiness." +
				" No one rejects, dislikes, or avoids pleasure itself, because it is pleasure," +
				" but because those who do not know how to pursue pleasure rationally encounter ",
				GenreName = "pop",
				Name = "The Eras Tour",
				Perfomer = "Taylor Swift",
				Place = place2,
            });
			await context.SaveChangesAsync();

            var sector = new Sector();
            await context.AddRangeAsync(new Sector
            {
                Name = SectorName.B,
                Price = TicketPriceCalculator.CalculatePrice((decimal)100.1, SectorName.B, place1.City),
                RowNumber = 16,
                RowSeatNumber = 14,
                Place = place1,
            },
            new Sector
            {
                Name = SectorName.DanceFloor,
                Price = TicketPriceCalculator.CalculatePrice((decimal)100.1, SectorName.DanceFloor, place1.City),
                RowNumber = 15,
                RowSeatNumber = 15,
                Place = place1,
            },
            new Sector
            {
                Name = SectorName.A,
                Price = TicketPriceCalculator.CalculatePrice((decimal)100.1, SectorName.A, place2.City),
                RowNumber = 0,
                RowSeatNumber = 55,
                Place = place2,
            },
            new Sector
            {
                Name = SectorName.A,
                Price = TicketPriceCalculator.CalculatePrice((decimal)100.1, SectorName.A, place2.City),
                RowNumber = 20,
                RowSeatNumber = 25,
                Place = place2,
            },
            new Sector
            {
                Name = SectorName.B,
                Price = TicketPriceCalculator.CalculatePrice((decimal)100.1, SectorName.B, place2.City),
                RowNumber = 20,
                RowSeatNumber = 25,
                Place = place2,
            });
            await context.SaveChangesAsync();

            await context.AddRangeAsync(new Ticket
            {
                Concert = context.Concerts.Where(s => s.Name == "POST HUMAN").Select(s => s).FirstOrDefault(),
                Sector = context.Sectors.Where(s => s.Name == SectorName.A).Select(s => s).FirstOrDefault(),
                StatusId = (int)StatusTypes.Free,
                Row = 1,
                Seat = 1,
            },
            new Ticket
            {
                Concert = context.Concerts.Where(s => s.Name == "Evolve Tour").Select(s => s).FirstOrDefault(),
                Sector = context.Sectors.Where(s => s.Name == SectorName.DanceFloor).Select(s => s).FirstOrDefault(),
                StatusId = (int)StatusTypes.Free,
                Row = 2,
                Seat = 2,
            },
            new Ticket
            {
                Concert = context.Concerts.Where(s => s.Name == "Evolve Tour").Select(s => s).FirstOrDefault(),
                Sector = context.Sectors.Where(s => s.Name == SectorName.DanceFloor).Select(s => s).FirstOrDefault(),
                StatusId = (int)StatusTypes.Free,
                Row = 10,
                Seat = 10,
            });
            await context.SaveChangesAsync();
        }
    }
}
