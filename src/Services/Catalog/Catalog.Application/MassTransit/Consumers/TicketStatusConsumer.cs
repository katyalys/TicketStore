using AutoMapper;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketService _basketService;

        public TicketStatusConsumer(IUnitOfWork unitOfWork, IBasketService basketService)
        {
            _unitOfWork = unitOfWork;
            _basketService = basketService;
        }

        public async Task Consume(ConsumeContext<GetTicketStatusEvent> context)
        {
            //foreach (var ticketId in context.Message.TicketBasketId)
            //{
            //    var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId);
            //    if (context.Message.TicketStatus == Shared.EventBus.Messages.Enums.Status.Paid)
            //    {
            //        ticket.StatusId = (int)StatusTypes.Bought;
            //    }
            //    else if (context.Message.TicketStatus == Shared.EventBus.Messages.Enums.Status.Canceled)
            //    {
            //        ticket.StatusId = (int)StatusTypes.Free;
            //    }

            //    _unitOfWork.Repository<Ticket>().Add(ticket);
            //    await _unitOfWork.Complete();
            //}

            //foreach (var ticketId in context.Message.TicketBasketId)
            //{
            //    var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId);
            //    if (context.Message.TicketStatus == Shared.EventBus.Messages.Enums.Status.Paid)
            //    {
            //        ticket.StatusId = (int)StatusTypes.Bought + 1;

            //        if (context.Message.UserId != null)
            //        {
            //            await _basketService.DeleteBasketAsync(context.Message.UserId);
            //        }
            //    }
            //    else if (context.Message.TicketStatus == Shared.EventBus.Messages.Enums.Status.Canceled)
            //    {
            //        ticket.StatusId = (int)StatusTypes.Free + 1;
            //        ticket.CustomerId = null;
            //    }

            //    _unitOfWork.Repository<Ticket>().Update(ticket);
            //}

            //await _unitOfWork.Complete();

            foreach (var ticketId in context.Message.TicketBasketId)
            {
                var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId);
                if (context.Message.TicketStatus == Shared.EventBus.Messages.Enums.Status.Paid)
                {
                    ticket.StatusId = (int)StatusTypes.Bought + 1;
                }
                else if (context.Message.TicketStatus == Shared.EventBus.Messages.Enums.Status.Canceled)
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

            //delete from redis
            //change status in db
        }
    }
}
