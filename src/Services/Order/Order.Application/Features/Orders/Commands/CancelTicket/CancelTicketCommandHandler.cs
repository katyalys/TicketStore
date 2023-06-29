using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Order.Application.Features.Orders.Commands.CancelTicket;
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
    public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Ticket> _ticketRepository;

        public CancelTicketCommandHandler(IMapper mapper, IGenericRepository<Ticket> ticketRepository)
        {
            _mapper = mapper;
            _ticketRepository = ticketRepository;
        }

        public async Task<bool> Handle(CancelTicketCommand request, CancellationToken cancellationToken)
        {
            var grpcRequest = new GetTicketDateRequest();
            grpcRequest.TicketId.Add(request.TicketId);

            using var channel = GrpcChannel.ForAddress("https://localhost:5045");
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = client.GetTicketDate(grpcRequest);
            var ticketDto = ticketOrderDto.TicketDate.FirstOrDefault();
            var date = DateTimeOffset.FromUnixTimeSeconds(ticketDto.Date.Seconds).DateTime;

            if (date < DateTime.Today.AddDays(10))
            {
                var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
                ticket.TicketStatus = Status.Canceled;
                _ticketRepository.Update(ticket);

                return true;
            }

            return false;

        }
    }
    
}
