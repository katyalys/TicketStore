using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Config
{
    public class SectorConfiguration
    {
		public void Configure(EntityTypeBuilder<Sector> builder)
		{
			builder.Property(d => d.Name).HasMaxLength(10).IsRequired();
			builder.Property(d => d.RowNumber).HasMaxLength(4).IsRequired();
			builder.Property(d => d.RowSeatNumber).HasMaxLength(5).IsRequired();
			builder.Property(d => d.Price).HasColumnType("money(10,4)").IsRequired();
			builder.HasMany(d => d.Tickets).WithOne(d => d.Sector);
		}
	}
}
