using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Configuration;
using Order.Application.Dtos;
using Order.Domain.Entities;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Domain.Specification.OrderSpecifications;
using Order.Infrastructure.Services;
using OrderClientGrpc;

namespace Order.Application.Features.Orders.Queries.AllOrders
{
    public class AllOrdersQueryHandler : IRequestHandler<AllOrdersQuery, Result<List<OrderDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly OrderProtoService.OrderProtoServiceClient _client;

        public AllOrdersQueryHandler(IMapper mapper, 
            IGenericRepository<OrderTicket> orderRepository,
            OrderProtoService.OrderProtoServiceClient client)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _client = client;
        }

        public async Task<Result<List<OrderDto>>> Handle(AllOrdersQuery request, CancellationToken cancellationToken)
        {
            var fullOrderDtoList = new List<OrderDto>();
            var getTicketSpec = new GetTicketsSpec();
            var orderInfoList = await _orderRepository.ListAsync(getTicketSpec);

            if (!orderInfoList.Any())
            {
                return ResultReturnService.CreateErrorResult<List<OrderDto>>(ErrorStatusCode.NotFound,
                    "No orders");
            }

            var orderGroups = orderInfoList.GroupBy(u => u.CustomerId);

            foreach (var orderGroup in orderGroups)
            {
                var customerId = orderGroup.Key;
                var ticketIds = orderGroup.SelectMany(u => u.Tickets)
                                         .Select(t => t.TicketBasketId)
                                         .ToList();

                if (!ticketIds.Any())
                {
                    return ResultReturnService.CreateErrorResult<List<OrderDto>>(ErrorStatusCode.NotFound,
                        "No tickets in order");
                }

                var grpcRequest = new GetTicketInfoRequest();
                grpcRequest.TicketId.Add(ticketIds);
                var ticketOrderDto = await _client.GetTicketInfoAsync(grpcRequest);

                if (ticketOrderDto == null || !ticketOrderDto.TicketDto.Any())
                {
                    return ResultReturnService.CreateErrorResult<List<OrderDto>>(ErrorStatusCode.NotFound,
                        "Cant get ticket info");
                }

                var ticketInfoList = new List<TicketDetailInfoDto>();

                foreach (var ticketDto in ticketOrderDto.TicketDto)
                {
                    var ticketInfo = _mapper.Map<TicketDetailInfoDto>(ticketDto);
                    ticketInfo.Date = _mapper.Map<DateTime>(ticketDto.Concert.Date);
                    ticketInfoList.Add(ticketInfo);
                }

                var fullOrderDto = _mapper.Map<OrderDto>(orderGroup.FirstOrDefault());
                fullOrderDto.CustomerId = customerId;
                fullOrderDto.TicketDetails = ticketInfoList;
                fullOrderDtoList.Add(fullOrderDto);
            }

            return new Result<List<OrderDto>>()
            {
                Value = fullOrderDtoList
            };
        }
    }
}
