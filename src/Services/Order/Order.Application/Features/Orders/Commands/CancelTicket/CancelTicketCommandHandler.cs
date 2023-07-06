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
using Order.Domain.Specification.TicketSpecifications;
using static Order.Application.Constants.Constants;
using Order.Infrastructure.Services;
using OrderClientGrpc;
using Shared.EventBus.Messages.Events;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand, Result>
    {
        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly string _url;
        private readonly GrpcChannel _channel;
        private readonly OrderProtoService.OrderProtoServiceClient _client;

        public CancelTicketCommandHandler(IGenericRepository<Ticket> ticketRepository, IConfiguration configuration,
                                            IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _ticketRepository = ticketRepository;
            _url = configuration["GrpcServer:Address"];
            _channel = GrpcChannel.ForAddress(_url);
            _client = new OrderProtoService.OrderProtoServiceClient(_channel);
        }

        public async Task<Result> Handle(CancelTicketCommand request, CancellationToken cancellationToken)
        {
            var spec = new CheckTicketExsistSpec(request.TicketId, request.OrderId, request.CustomerId);
            var ticketExists = await _ticketRepository.ListAsync(spec);

            if (!ticketExists.Any())
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction,
                    "Check input data or tickets already canceled");
            }

            var ticketIds = ticketExists.Select(ticket => ticket.TicketBasketId).FirstOrDefault();
            var grpcRequest = new GetTicketDateRequest();
            grpcRequest.TicketId.Add(ticketIds);
            var ticketOrderDto = await _client.GetTicketDateAsync(grpcRequest);

            if (ticketOrderDto == null)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound,
                    "Ticket info not found");
            }

            var ticketDto = ticketOrderDto.TicketDate.FirstOrDefault();
            var concertDate = DateTimeOffset.FromUnixTimeSeconds(ticketDto.Date.Seconds).DateTime;

            if (concertDate > DateTime.Today.AddDays(DaysAheadTicketReturn))
            {
                var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

                if (ticket == null)
                {
                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound,
                        "Cant find ticket");
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
