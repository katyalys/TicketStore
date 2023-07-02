using AutoMapper;
using MediatR;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;
using OrderClientGrpc;
using Grpc.Net.Client;
using Order.Infrastructure.Data;
using Order.Domain.Enums;
using Order.Application.Dtos;
using Order.Domain.ErrorModels;
using Order.Infrastructure.Services;

namespace Order.Application.Features.Orders.Queries.TicketsDetailedInfo
{
    public class TicketDetailedQueryHandler : IRequestHandler<TicketsDetailedQuery, Result<List<TicketDetailInfo>>>
    {
        private readonly IMapper _mapper;
        private readonly OrderContext _orderContext;

        public TicketDetailedQueryHandler(IMapper mapper, OrderContext orderContext)
        {
            _mapper = mapper;
            _orderContext = orderContext;
        }

        public async Task<Result<List<TicketDetailInfo>>> Handle(TicketsDetailedQuery request, CancellationToken cancellationToken)
        {
            var ticketIds = _orderContext.Orders.Where(u => u.Id == request.OrderId
                                                    && u.CustomerId == request.CustomerId
                                                    && u.OrderStatus != Status.Canceled)
                                            .SelectMany(u => u.Tickets)
                                            .Where(t => t.TicketStatus == Status.Paid)
                                            .Select(t => t.TicketBasketId)
                                            .ToList();

            if (ticketIds.Count() == 0)
            {
                return ResultReturnService.CreateErrorResult<List<TicketDetailInfo>>(ErrorStatusCode.NotFound, "No tickets were found");
            }

            var grpcRequest = new GetTicketInfoRequest();
            grpcRequest.TicketId.Add(ticketIds);

            using var channel = GrpcChannel.ForAddress("https://localhost:5046");
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = await client.GetTicketInfoAsync(grpcRequest);

            if (ticketOrderDto == null || ticketOrderDto.TicketDto.Count() == 0)
            {
                return ResultReturnService.CreateErrorResult<List<TicketDetailInfo>>(ErrorStatusCode.NotFound, "No tickets were found");
            }

            List<TicketDetailInfo> ticketInfoList = new List<TicketDetailInfo>();
            foreach (var ticketDto in ticketOrderDto.TicketDto)
            {
                var ticketInfo = _mapper.Map<TicketDetailInfo>(ticketDto);
                ticketInfo.Date = _mapper.Map<DateTime>(ticketDto.Concert.Date);
                ticketInfoList.Add(ticketInfo);
            }

            return new Result<List<TicketDetailInfo>>()
            {
                Value = ticketInfoList
            };
        }
    }
}
