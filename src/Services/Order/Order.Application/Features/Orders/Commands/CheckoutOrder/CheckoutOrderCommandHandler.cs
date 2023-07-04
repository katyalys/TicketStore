using AutoMapper;
using Grpc.Net.Client;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Infrastructure.Services;
using OrderClientGrpc;
using Shared.EventBus.Messages.Events;

namespace Order.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly string _url;
        private readonly IPublishEndpoint _publishEndpoint;

        public CheckoutOrderCommandHandler(IMapper mapper, IGenericRepository<OrderTicket> orderRepository, IConfiguration configuration, IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _url = configuration["GrpcServer:Address"];
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<int>> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = await client.GetTicketsToOrderAsync(new GetTicketsRequest { UserId = request.CustomerId });

            if (ticketOrderDto == null)
            {
                return ResultReturnService.CreateErrorResult<int>(ErrorStatusCode.NotFound, "No tickets to checkout");
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

            await _orderRepository.Add(order);
            await _orderRepository.SaveAsync();

            // send checkout event to rabbitmq
            var eventMessage = _mapper.Map<GetTicketStatusEvent>(ticketOrderDto);
            eventMessage.TicketStatus = Shared.EventBus.Messages.Enums.Status.Paid;
           // eventMessage.UserId = request.CustomerId;
            await _publishEndpoint.Publish(eventMessage);

            return new Result<int>()
            {
                Value = order.Id
            };
        }
    }
}
