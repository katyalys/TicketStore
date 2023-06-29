using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Interfaces;
using OrderClientGrpc;

namespace Order.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;

        public CheckoutOrderCommandHandler(IMapper mapper, IGenericRepository<OrderTicket> orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5046");
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = client.GetTicketsToOrder(new GetTicketsRequest { UserId = request.CustomerId });
            var order = _mapper.Map<OrderTicket>(ticketOrderDto);
            order.OrderDate = DateTime.UtcNow;
            order.OrderStatus = Status.Paid;

            order.Tickets = ticketOrderDto.TicketIds.Select(ticketId => new Ticket
            {
                TicketBasketId = ticketId,
                //Order = order
            }).ToList();

            await _orderRepository.Add(order);
            await _orderRepository.SaveAsync();


            //foreach (var ticketId in ticketOrderDto.TicketIds) 
            //{
            //    Ticket ticket = new Ticket();
            //    ticket.TicketBasketId = ticketId;
            //    ticket.OrderTicketId = order.Id;
            //    order.Tickets.Add(ticket);
            //}
            // var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return order.Id;
        }
    }
}
