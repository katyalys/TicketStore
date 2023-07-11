using MediatR;
using Order.Application.Features.Orders.Commands.CancelTicket;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Domain.Specification.TicketSpecifications;
using static Order.Application.Constants.Constants;
using Order.Infrastructure.Services;
using OrderClientGrpc;
using Microsoft.Extensions.Logging;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand, Result>
    {
        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly OrderProtoService.OrderProtoServiceClient _client;
        private readonly ILogger<CancelTicketCommandHandler> _logger;

        public CancelTicketCommandHandler(IGenericRepository<Ticket> ticketRepository, 
            OrderProtoService.OrderProtoServiceClient client,
            ILogger<CancelTicketCommandHandler> logger)
        {
            _ticketRepository = ticketRepository;
            _client = client;
            _logger = logger;
        }

        public async Task<Result> Handle(CancelTicketCommand request, CancellationToken cancellationToken)
        {
            var spec = new CheckTicketExsistSpec(request.TicketId, request.OrderId, request.CustomerId);
            var ticketExists = await _ticketRepository.ListAsync(spec);

            if (!ticketExists.Any())
            {
                _logger.LogError("Invalid input data or tickets already canceled. TicketId: {TicketId}, OrderId: {OrderId}, CustomerId: {CustomerId}", 
                    request.TicketId, request.OrderId, request.CustomerId);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction,
                    "Check input data or tickets already canceled");
            }

            var ticketIds = ticketExists.Select(ticket => ticket.TicketBasketId).FirstOrDefault();
            var grpcRequest = new GetTicketDateRequest();
            grpcRequest.TicketId.Add(ticketIds);
            var ticketOrderDto = await _client.GetTicketDateAsync(grpcRequest);

            if (ticketOrderDto == null)
            {
                _logger.LogError("Ticket info not found for TicketId {TicketId}, OrderId {OrderId}, CustomerId {CustomerId}", 
                    request.TicketId, request.OrderId, request.CustomerId);

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
                    _logger.LogError("Ticket not found for TicketId {TicketId}, OrderId {OrderId}, CustomerId {CustomerId}", 
                        request.TicketId, request.OrderId, request.CustomerId);

                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound,
                        "Cant find ticket");
                }

                ticket.TicketStatus = Status.Canceled;
                _ticketRepository.Update(ticket);
                await _ticketRepository.SaveAsync();
            }

            _logger.LogInformation("Ticket canceled successfully. TicketId: {TicketId}, OrderId: {OrderId}, CustomerId: {CustomerId}",
                request.TicketId, request.OrderId, request.CustomerId);

            return ResultReturnService.CreateSuccessResult();
        }
    }
}
