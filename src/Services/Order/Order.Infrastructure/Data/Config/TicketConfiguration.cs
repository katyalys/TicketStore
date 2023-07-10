using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Data.Config
{
    public class TicketConfiguration
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.Property(d => d.TicketBasketId).IsRequired();
            builder.Property(d => d.TicketStatus).IsRequired();
            builder.HasOne(d => d.Order).WithMany(x => x.Tickets).HasForeignKey(x => x.OrderTicketId);
        }
    }
}
