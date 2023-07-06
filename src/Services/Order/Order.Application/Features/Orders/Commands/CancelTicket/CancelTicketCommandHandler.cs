using AutoMapper;
using Grpc.Net.Client;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Order.Application.Features.Orders.Commands.CancelTicket;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Services;
using OrderClientGrpc;
using Shared.EventBus.Messages.Events;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand, Result>
    {
        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly OrderContext _orderContext;
        private readonly string _url;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public CancelTicketCommandHandler(IGenericRepository<Ticket> ticketRepository, OrderContext orderContext, IConfiguration configuration,
                                            IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _ticketRepository = ticketRepository;
            _orderContext = orderContext;
            _url = configuration["GrpcServer:Address"];
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result> Handle(CancelTicketCommand request, CancellationToken cancellationToken)
        {
            var ticketExists = _orderContext.Tickets.Where(u => u.OrderTicketId == request.OrderId
                                                           && u.Id == request.TicketId
                                                           && u.TicketStatus == Status.Paid
                                                           && request.CustomerId == request.CustomerId);

            if (ticketExists.Count() == 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Check input data or tickets already canceled");
            }

            var ticketIds = ticketExists.Select(ticket => ticket.TicketBasketId).FirstOrDefault();
            var grpcRequest = new GetTicketDateRequest();
            grpcRequest.TicketId.Add(ticketIds);

            using var channel = GrpcChannel.ForAddress(_url);
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = await client.GetTicketDateAsync(grpcRequest);

            if (ticketOrderDto == null)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "Ticket info not found");
            }

            var ticketDto = ticketOrderDto.TicketDate.FirstOrDefault();
            var concertDate = DateTimeOffset.FromUnixTimeSeconds(ticketDto.Date.Seconds).DateTime;
            if (concertDate > DateTime.Today.AddDays(10))
            {
                var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

                if (ticket == null)
                {
                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "Cant find ticket");
                }

                ticket.TicketStatus = Status.Canceled;
                _ticketRepository.Update(ticket);
                await _ticketRepository.SaveAsync();
            }

            var eventMessage = _mapper.Map<GetTicketStatusEvent>(grpcRequest);
            eventMessage.TicketStatus = Shared.EventBus.Messages.Enums.Status.Canceled;
            await _publishEndpoint.Publish(eventMessage);

            return ResultReturnService.CreateSuccessResult();
        }
    }

}
