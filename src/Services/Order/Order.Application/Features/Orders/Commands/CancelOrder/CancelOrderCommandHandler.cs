using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Interfaces;
using OrderClientGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly IGenericRepository<Ticket> _ticketRepository;

        public CancelOrderCommandHandler(IMapper mapper, IGenericRepository<OrderTicket> orderRepository, IGenericRepository<Ticket> ticketRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            List<int> ticketIds = order.Tickets.Select(ticket => ticket.Id).ToList();
            var grpcRequest = new GetTicketDateRequest();
            grpcRequest.TicketId.AddRange(ticketIds);

            using var channel = GrpcChannel.ForAddress("https://localhost:5045");
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = client.GetTicketDate(grpcRequest);

            foreach (var ticketDto in ticketOrderDto.TicketDate)
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(ticketDto.Date.Seconds).DateTime;

                if (date < DateTime.Today.AddDays(10))
                {
                    var ticket = await _ticketRepository.GetByIdAsync(ticketDto.TicketId);
                    ticket.TicketStatus = Status.Canceled;
                    _ticketRepository.Update(ticket);
                }
                else
                {
                    return false;
                }
            }

            return true;

        }
    }
}
