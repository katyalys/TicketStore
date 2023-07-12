using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly OrderProtoService.OrderProtoServiceClient _client;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(IMapper mapper, 
            IGenericRepository<OrderTicket> orderRepository,
            OrderProtoService.OrderProtoServiceClient client,
            ILogger<CheckoutOrderCommandHandler> logger)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _client = client;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var ticketOrderDto = await _client.GetTicketsToOrderAsync(new GetTicketsRequest { UserId = request.CustomerId });

            if (ticketOrderDto == null)
            {
                _logger.LogWarning("No tickets to checkout for CustomerId {CustomerId}", request.CustomerId);

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

            _logger.LogInformation("Order successfully checked out. OrderId: {OrderId}, CustomerId: {CustomerId}", 
                order.Id, request.CustomerId);

            return new Result<int>()
            {
                Value = order.Id
            };
        }
    }
}
