using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Catalog.Infrastructure.Data
{
    public class CatalogContext: DbContext
    {
		public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
		{
		}
		public DbSet<Concert> Concerts { get; set; }
		public DbSet<Place> Places { get; set; }
		public DbSet<Sector> Sectors { get; set; }
		public DbSet<Status> Statuses { get; set; }
		public DbSet<Ticket> Tickets { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
