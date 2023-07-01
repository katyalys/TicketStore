using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Order.Application.Features.Orders.Commands.CancelTicket;
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
    public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly OrderContext _orderContext;

        public CancelTicketCommandHandler(IMapper mapper, IGenericRepository<Ticket> ticketRepository,
                        IGenericRepository<OrderTicket> orderRepository, OrderContext orderContext)
        {
            _mapper = mapper;
            _ticketRepository = ticketRepository;
            _orderRepository = orderRepository;
            _orderContext = orderContext;
        }

        public async Task<bool> Handle(CancelTicketCommand request, CancellationToken cancellationToken)
        {
            var ticketExists = _orderContext.Tickets.Where(u => u.OrderTicketId == request.OrderId
                                                           && u.Id == request.TicketId 
                                                           && request.CustomerId == request.CustomerId);

            if (ticketExists == null)
            {
                throw new Exception("Check input data");
            }

            var ticketIds = ticketExists.Select(ticket => ticket.TicketBasketId).FirstOrDefault();
            var grpcRequest = new GetTicketDateRequest();
            grpcRequest.TicketId.Add(ticketIds);

            using var channel = GrpcChannel.ForAddress("https://localhost:5046");
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = await client.GetTicketDateAsync(grpcRequest);
            var ticketDto = ticketOrderDto.TicketDate.FirstOrDefault();
           // var date = DateTimeOffset.FromUnixTimeSeconds(ticketDto.Date.Seconds).DateTime;

            var concertDate = DateTimeOffset.FromUnixTimeSeconds(ticketDto.Date.Seconds).DateTime;

            if (concertDate > DateTime.Today.AddDays(10))
            {
            //    if (date < DateTime.Today.AddDays(10))
            //{
                var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
                ticket.TicketStatus = Status.Canceled;
                _ticketRepository.Update(ticket);
                await _ticketRepository.SaveAsync();

                return true;
            }

            return false;

        }
    }
    
}
