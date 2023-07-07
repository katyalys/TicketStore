using MediatR;
using AutoMapper;
using MassTransit;
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
using Shared.EventBus.Messages.Enums;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand, Result>
    {
        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly OrderProtoService.OrderProtoServiceClient _client;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public CancelTicketCommandHandler(IGenericRepository<Ticket> ticketRepository,
            IMapper mapper,
            OrderProtoService.OrderProtoServiceClient client,
            IPublishEndpoint publishEndpoint)
        {
            _ticketRepository = ticketRepository;
            _client = client;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
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
            eventMessage.TicketStatus = MessageStatus.Canceled;
            await _publishEndpoint.Publish(eventMessage);

            return ResultReturnService.CreateSuccessResult();
        }
    }
}
