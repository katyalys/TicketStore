using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MassTransit;
using Shared.EventBus.Messages.Events;

namespace Catalog.Application.MassTransit.Consumers
{
    public class TicketStatusConsumer : IConsumer<GetTicketStatusEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketService _basketService;

        public TicketStatusConsumer(IUnitOfWork unitOfWork, IBasketService basketService)
        {
            _unitOfWork = unitOfWork;
            _basketService = basketService;
        }

        public async Task Consume(ConsumeContext<GetTicketStatusEvent> context)
        {
            foreach (var ticketId in context.Message.TicketBasketId)
            {
                var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId);

                if (context.Message.TicketStatus == Shared.EventBus.Messages.Enums.Status.Paid)
                {
                    ticket.StatusId = (int)StatusTypes.Bought + 1;
                }
                else
                {
                    ticket.StatusId = (int)StatusTypes.Free + 1;
                    ticket.CustomerId = null;
                }

                _unitOfWork.Repository<Ticket>().Update(ticket);
            }

            await _unitOfWork.Complete();

            if (context.Message.TicketStatus == Shared.EventBus.Messages.Enums.Status.Paid && context.Message.UserId != null)
            {
                await _basketService.DeleteBasketAsync(context.Message.UserId);
            }
        }
    }
}
