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
using Microsoft.Extensions.Configuration;

namespace Order.Application.Features.Orders.Queries.TicketsDetailedInfo
{
    public class TicketDetailedQueryHandler : IRequestHandler<TicketsDetailedQuery, Result<List<TicketDetailInfoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly OrderContext _orderContext;
        private readonly string _url;

        public TicketDetailedQueryHandler(IMapper mapper, OrderContext orderContext, IConfiguration configuration)
        {
            _mapper = mapper;
            _orderContext = orderContext;
            _url = configuration["GrpcServer:Address"];
        }

        public async Task<Result<List<TicketDetailInfoDto>>> Handle(TicketsDetailedQuery request, CancellationToken cancellationToken)
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
                return ResultReturnService.CreateErrorResult<List<TicketDetailInfoDto>>(ErrorStatusCode.NotFound, "No tickets were found");
            }

            var grpcRequest = new GetTicketInfoRequest();
            grpcRequest.TicketId.Add(ticketIds);

            using var channel = GrpcChannel.ForAddress(_url);
            var client = new OrderProtoService.OrderProtoServiceClient(channel);
            var ticketOrderDto = await client.GetTicketInfoAsync(grpcRequest);

            if (ticketOrderDto == null || ticketOrderDto.TicketDto.Count() == 0)
            {
                return ResultReturnService.CreateErrorResult<List<TicketDetailInfoDto>>(ErrorStatusCode.NotFound, "No tickets were found");
            }

            List<TicketDetailInfoDto> ticketInfoList = new List<TicketDetailInfoDto>();
            foreach (var ticketDto in ticketOrderDto.TicketDto)
            {
                var ticketInfo = _mapper.Map<TicketDetailInfoDto>(ticketDto);
                ticketInfo.Date = _mapper.Map<DateTime>(ticketDto.Concert.Date);
                ticketInfoList.Add(ticketInfo);
            }

            return new Result<List<TicketDetailInfoDto>>()
            {
                Value = ticketInfoList
            };
        }
    }
}
