using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Infrastructure.Data.Config
{
    public class TicketConfiguration
    {
		public void Configure(EntityTypeBuilder<Ticket> builder)
		{
			builder.Property(d => d.TicketBasketId).IsRequired();
			builder.Property(d => d.Row).HasMaxLength(4);
			builder.Property(d => d.Seat).HasMaxLength(4);
			builder.Property(d => d.TicketStatus).IsRequired();
			builder.Property(d => d.Sector).HasMaxLength(10);
			builder.Property(d => d.Date).HasColumnType("datetime2");
			builder.Property(d => d.Concert).HasMaxLength(50);
			builder.Property(d => d.Place).HasMaxLength(50);
			builder.HasOne(d => d.Order).WithMany(x => x.Tickets).HasForeignKey(x => x.OrderTicketId);
		}
	}
}
