using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Configuration;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Services;
using OrderClientGrpc;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly OrderContext _orderContext;
        private readonly string _url;

        public CancelOrderCommandHandler(IGenericRepository<OrderTicket> orderRepository, IGenericRepository<Ticket> ticketRepository, 
                                            OrderContext orderContext, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _ticketRepository = ticketRepository;
            _orderContext = orderContext;
            _url = configuration["GrpcServer:Address"];
        }

        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order.CustomerId != request.CustomerId || order == null)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Invalid input values");
            }

            var ticketIds = _orderContext.Tickets.Where(u => u.OrderTicketId == request.OrderId).Select(ticket => ticket.TicketBasketId).ToList();
            if (ticketIds.Count() == 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "No tickets to checkout");
            }

            var grpcRequest = new GetTicketDateRequest();
            grpcRequest.TicketId.AddRange(ticketIds);

            using var channel = GrpcChannel.ForAddress(_url);
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
                                                        .Select(id => id.Id).FirstOrDefault();
                    var ticket = await _ticketRepository.GetByIdAsync(ticketId);

                    if (ticket == null)
                    {
                        return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "Cant cancel ticket or tickets have been already canceled");
                    }

                    ticket.TicketStatus = Status.Canceled;
                    _ticketRepository.Update(ticket);
                    await _ticketRepository.SaveAsync();
                }
                else
                {
                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "Tickets are canceled 10 days before the show max");
                }
            }

            order.OrderStatus = Status.Canceled;
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();

            return ResultReturnService.CreateSuccessResult();
        }
    }
}
