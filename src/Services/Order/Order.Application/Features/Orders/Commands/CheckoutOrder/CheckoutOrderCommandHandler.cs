using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Configuration;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Infrastructure.Services;
using OrderClientGrpc;

namespace Order.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly string _url;
        private readonly GrpcChannel _channel;
        private readonly OrderProtoService.OrderProtoServiceClient _client;

        public CheckoutOrderCommandHandler(IMapper mapper, IGenericRepository<OrderTicket> orderRepository, IConfiguration configuration)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _url = configuration["GrpcServer:Address"];
            _channel = GrpcChannel.ForAddress(_url);
            _client = new OrderProtoService.OrderProtoServiceClient(_channel);
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

            return new Result<int>()
            {
                Value = order.Id
            };
        }
    }
}
