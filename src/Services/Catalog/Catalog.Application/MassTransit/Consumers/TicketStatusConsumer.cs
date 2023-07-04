using MassTransit;
using Shared.EventBus.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.MassTransit.Consumers
{
    public class TicketStatusConsumer : IConsumer<GetTicketStatusEvent>
    {
        public TicketStatusConsumer()
        {

        }

        public async Task Consume(ConsumeContext<GetTicketStatusEvent> context)
        {
            //delete from redis
            //change status in db
        }
    }
}
