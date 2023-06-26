using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data.Config
{
    public class TicketConfiguration
    {
		public void Configure(EntityTypeBuilder<Ticket> builder)
		{
			builder.Property(d => d.Row).HasMaxLength(4).IsRequired();
			builder.Property(d => d.Seat).HasMaxLength(4).IsRequired();
		}
	}
}
