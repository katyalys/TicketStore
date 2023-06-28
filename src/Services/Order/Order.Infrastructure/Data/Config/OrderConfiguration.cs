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
    public class OrderConfiguration
    {
		public void Configure(EntityTypeBuilder<OrderTicket> builder)
		{
			builder.Property(d => d.OrderDate).HasColumnType("datetime2").IsRequired();
			builder.Property(d => d.TotalPrice).HasColumnType("money(10,4)").IsRequired();
			builder.Property(d => d.OrderStatus).IsRequired();
			builder.Property(d => d.CustomerId).IsRequired();
		}
	}
}
