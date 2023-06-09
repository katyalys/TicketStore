﻿using AutoMapper;
using MassTransit;
using MediatR;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Domain.Specification.TicketSpecifications;
using Order.Infrastructure.Services;
using static Order.Application.Constants.Constants;
using OrderClientGrpc;
using Shared.EventBus.Messages.Events;
using Shared.EventBus.Messages.Enums;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly OrderProtoService.OrderProtoServiceClient _client;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public CancelOrderCommandHandler(IGenericRepository<OrderTicket> orderRepository,
            IMapper mapper,
            IGenericRepository<Ticket> ticketRepository,
            OrderProtoService.OrderProtoServiceClient client,
            IPublishEndpoint publishEndpoint)
        {
            _orderRepository = orderRepository;
            _ticketRepository = ticketRepository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
            _client = client;
        }

        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order.CustomerId != request.CustomerId || order == null)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction,
                    "Invalid input values");
            }

            var ticketByOrderSpec = new TicketsByOrderSpec(request.OrderId);
            var tickets = await _ticketRepository.ListAsync(ticketByOrderSpec);

            if (!tickets.Any())
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound,
                    "No tickets to checkout");
            }

            var grpcRequest = new GetTicketDateRequest();
            var ticketIds = tickets.Select(ticket => ticket.TicketBasketId).ToList();
            grpcRequest.TicketId.AddRange(ticketIds);
            var ticketOrderDto = await _client.GetTicketDateAsync(grpcRequest);

            foreach (var ticketDto in ticketOrderDto.TicketDate)
            {
                var concertDate = DateTimeOffset.FromUnixTimeSeconds(ticketDto.Date.Seconds).DateTime;

                if (concertDate > DateTime.Today.AddDays(DaysAheadTicketReturn))
                {
                    var ticketSpec = new TicketSpec(ticketDto.TicketId, request.OrderId);
                    var ticket = await _ticketRepository.GetEntityWithSpec(ticketSpec);

                    if (ticket == null)
                    {
                        return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound,
                            "Cant cancel ticket or tickets have been already canceled");
                    }

                    ticket.TicketStatus = Status.Canceled;
                    _ticketRepository.Update(ticket);
                    await _ticketRepository.SaveAsync();
                }
                else
                {
                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound,
                        "Tickets are canceled 10 days before the show max");
                }
            }

            order.OrderStatus = Status.Canceled;
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();

            var eventMessage = _mapper.Map<GetTicketStatusEvent>(grpcRequest);
            eventMessage.TicketStatus = MessageStatus.Canceled;
            await _publishEndpoint.Publish(eventMessage);

            return ResultReturnService.CreateSuccessResult();
        }
    }
}
