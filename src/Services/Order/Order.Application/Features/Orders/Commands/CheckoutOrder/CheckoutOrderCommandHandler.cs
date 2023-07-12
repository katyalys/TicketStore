using AutoMapper;
using MassTransit;
using MediatR;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Infrastructure.Services;
using OrderClientGrpc;
using Shared.EventBus.Messages.Enums;
using Shared.EventBus.Messages.Events;

namespace Order.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly OrderProtoService.OrderProtoServiceClient _client;
        private readonly IPublishEndpoint _publishEndpoint;

        public CheckoutOrderCommandHandler(IMapper mapper,
            IGenericRepository<OrderTicket> orderRepository,
            OrderProtoService.OrderProtoServiceClient client,
            IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _client = client;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<int>> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var ticketOrderDto = await _client.GetTicketsToOrderAsync(new GetTicketsRequest { UserId = request.CustomerId });

            if (ticketOrderDto == null)
            {
                return ResultReturnService.CreateErrorResult<int>(ErrorStatusCode.NotFound,
                    "No tickets to checkout");
            }

            var order = _mapper.Map<OrderTicket>(ticketOrderDto);
            order.CustomerId = request.CustomerId;
            order.OrderDate = DateTime.UtcNow;
            order.OrderStatus = Status.Paid;

            order.Tickets = ticketOrderDto.TicketIds.Select(ticketId => new Ticket
            {
                TicketBasketId = ticketId,
                TicketStatus = Status.Paid,
            }).ToList();

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveAsync();

            var eventMessage = _mapper.Map<GetTicketStatusEvent>(ticketOrderDto);
            eventMessage.TicketStatus = MessageStatus.Paid;
            await _publishEndpoint.Publish(eventMessage);

            return new Result<int>()
            {
                Value = order.Id
            };
        }
    }
}
