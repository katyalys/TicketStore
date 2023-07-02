using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Order.Application.Dtos;
using Order.Domain.ErrorModels;
using Order.Infrastructure.Data;
using Order.Infrastructure.Services;
using OrderClientGrpc;

namespace Order.Application.Features.Orders.Queries.OrderHistory
{
    public class OrderHistoryQueryHandler : IRequestHandler<OrderHistoryQuery, Result<List<FullOrderDto>>>
    {
        private readonly IMapper _mapper;
        private readonly OrderContext _orderContext;

        public OrderHistoryQueryHandler(IMapper mapper, OrderContext orderContext)
        {
            _mapper = mapper;
            _orderContext = orderContext;
        }

        public async Task<Result<List<FullOrderDto>>> Handle(OrderHistoryQuery request, CancellationToken cancellationToken)
        {
            var fullOrderDtoList = new List<FullOrderDto>();
            var orderInfoList = _orderContext.Orders.Where(u => u.CustomerId == request.CustomerId);

            if (orderInfoList.Count() == 0)
            {
                return ResultReturnService.CreateErrorResult<List<FullOrderDto>>(ErrorStatusCode.ForbiddenAction, "Unauthorized access");
            }

            foreach (var orderInfo in orderInfoList)
            {
                var ticketIds = orderInfoList.SelectMany(u => u.Tickets)
                                         .Select(t => t.TicketBasketId)
                                         .ToList();

                if (ticketIds.Count() == 0)
                {
                    return ResultReturnService.CreateErrorResult<List<FullOrderDto>>(ErrorStatusCode.NotFound, "No tickets in orders");
                }

                var grpcRequest = new GetTicketInfoRequest();
                grpcRequest.TicketId.Add(ticketIds);

                using var channel = GrpcChannel.ForAddress("https://localhost:5046");
                var client = new OrderProtoService.OrderProtoServiceClient(channel);
                var ticketOrderDto = await client.GetTicketInfoAsync(grpcRequest);

                if (ticketOrderDto == null)
                {
                    return ResultReturnService.CreateErrorResult<List<FullOrderDto>>(ErrorStatusCode.NotFound, "No tickets were found");
                }

                List<TicketDetailInfo> ticketInfoList = new List<TicketDetailInfo>();
                foreach (var ticketDto in ticketOrderDto.TicketDto)
                {
                    var ticketInfo = _mapper.Map<TicketDetailInfo>(ticketDto);
                    ticketInfo.Date = _mapper.Map<DateTime>(ticketDto.Concert.Date);
                    ticketInfoList.Add(ticketInfo);
                }

                var fullOrderDto = _mapper.Map<FullOrderDto>(orderInfo);
                fullOrderDto.TicketDetails = ticketInfoList;
                fullOrderDtoList.Add(fullOrderDto);
            }

            return new Result<List<FullOrderDto>>()
            {
                Value = fullOrderDtoList
            };
        }
    }
}
