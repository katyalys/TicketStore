using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Config
{
    public class StatusConfiguration
    {
		public void Configure(EntityTypeBuilder<Status> builder)
		{
			builder.Property(d => d.StatusType).HasMaxLength(10).IsRequired();
			builder.HasMany(d => d.Tickets).WithOne(d => d.Status);
		}
	}
}
