using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
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
        private readonly OrderContext _orderContext;

        public CancelOrderCommandHandler(IMapper mapper, IGenericRepository<OrderTicket> orderRepository, IGenericRepository<Ticket> ticketRepository, OrderContext orderContext)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _ticketRepository = ticketRepository;
            _orderContext = orderContext;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order.CustomerId != request.CustomerId)
            {
                throw new Exception("Unauthorized access");
            }

            var ticketIds = _orderContext.Tickets.Where(u => u.OrderTicketId == request.OrderId).Select(ticket => ticket.TicketBasketId).ToList();
            var grpcRequest = new GetTicketDateRequest();
            grpcRequest.TicketId.AddRange(ticketIds);

            using var channel = GrpcChannel.ForAddress("https://localhost:5046");
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = await client.GetTicketDateAsync(grpcRequest);

            foreach (var ticketDto in ticketOrderDto.TicketDate)
            {
                var concertDate = DateTimeOffset.FromUnixTimeSeconds(ticketDto.Date.Seconds).DateTime;

                if (concertDate > DateTime.Today.AddDays(10))
                {
                    var ticketId = _orderContext.Tickets.Where(u => u.TicketBasketId == ticketDto.TicketId 
                                                            && u.TicketStatus != Status.Canceled 
                                                            && u.OrderTicketId == request.OrderId)
                                                        .Select(id => id.Id).FirstOrDefault(); //else exception
                    var ticket = await _ticketRepository.GetByIdAsync(ticketId);
                    ticket.TicketStatus = Status.Canceled;
                    _ticketRepository.Update(ticket);
                    await _ticketRepository.SaveAsync();
                }
                else
                {
                    return false;
                }
            }

            order.OrderStatus = Status.Canceled;
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();

            return true;

        }
    }
}
