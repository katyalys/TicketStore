using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data.Config
{
    public class PlaceConfiguration
    {
		public void Configure(EntityTypeBuilder<Place> builder)
		{
			builder.Property(d => d.City).HasMaxLength(20).IsRequired();
			builder.Property(d => d.Street).HasMaxLength(30).IsRequired();
			builder.Property(d => d.PlaceNumber).HasMaxLength(5).IsRequired();
			builder.Property(d => d.Street).HasMaxLength(30).IsRequired();
			builder.Property(d => d.IsDeleted).HasDefaultValue(false).IsRequired();
		//	builder.HasMany(d => d.Concerts).WithOne(d => d.Place);
			builder.HasMany(d => d.Sectors).WithOne(d => d.Place);
		}
	}
}
