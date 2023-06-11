using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Config
{
	public class ProductConfiguration : IEntityTypeConfiguration<Concert>
	{
		public void Configure(EntityTypeBuilder<Concert> builder)
		{
			builder.Property(d => d.Date).IsRequired();
			builder.Property(d => d.Description).HasMaxLength(550).IsRequired();
			builder.Property(d => d.Name).HasMaxLength(50).IsRequired();
			builder.Property(d => d.Perfomer).HasMaxLength(30).IsRequired();
			builder.Property(d => d.GenreName).HasMaxLength(20).IsRequired();
			builder.HasMany(d => d.Tickets).WithOne(d => d.Concert);
			builder.HasOne(d => d.Place).WithMany(x => x.Concerts).HasForeignKey(x => x.PlaceId);
			builder.Property(d => d.IsDeleted).HasDefaultValue(false).IsRequired();
		}
	}
}
