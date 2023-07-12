using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Application.Dtos;
using Order.Domain.Entities;
using Order.Domain.ErrorModels;
using Order.Domain.Interfaces;
using Order.Domain.Specification.OrderSpecifications;
using Order.Infrastructure.Services;
using OrderClientGrpc;

namespace Order.Application.Features.Orders.Queries.OrderHistory
{
    public class OrderHistoryQueryHandler : IRequestHandler<OrderHistoryQuery, Result<List<FullOrderDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly OrderProtoService.OrderProtoServiceClient _client;
        private readonly ILogger<OrderHistoryQueryHandler> _logger;

        public OrderHistoryQueryHandler(IMapper mapper,
             OrderProtoService.OrderProtoServiceClient client,
             IGenericRepository<OrderTicket> orderRepository,
             ILogger<OrderHistoryQueryHandler> logger)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _client = client;
            _logger = logger;
        }

        public async Task<Result<List<FullOrderDto>>> Handle(OrderHistoryQuery request, CancellationToken cancellationToken)
        {
            var fullOrderDtoList = new List<FullOrderDto>();
            var spec = new OrderByCustomerSpec(request.CustomerId);
            var orderInfoList = await _orderRepository.ListAsync(spec);

            if (!orderInfoList.Any())
            {
                _logger.LogWarning("No orders found for CustomerId {CustomerId}", request.CustomerId);

                return ResultReturnService.CreateErrorResult<List<FullOrderDto>>(ErrorStatusCode.ForbiddenAction, "Unauthorized access");
            }

            foreach (var orderInfo in orderInfoList)
            {
                var ticketIds = orderInfo.Tickets.Select(t => t.TicketBasketId).ToList();
                var grpcRequest = new GetTicketInfoRequest();
                grpcRequest.TicketId.Add(ticketIds);
                var ticketOrderDto = await _client.GetTicketInfoAsync(grpcRequest);

                if (ticketOrderDto == null)
                {
                    _logger.LogWarning("No tickets found for order with Id {OrderId}", orderInfo.Id);

                    return ResultReturnService.CreateErrorResult<List<FullOrderDto>>(ErrorStatusCode.NotFound, "No tickets were found");
                }

                var ticketInfoList = new List<TicketDetailInfoDto>();

                foreach (var ticketDto in ticketOrderDto.TicketDto)
                {
                    var ticketInfo = _mapper.Map<TicketDetailInfoDto>(ticketDto);
                    ticketInfo.Date = _mapper.Map<DateTime>(ticketDto.Concert.Date);
                    ticketInfoList.Add(ticketInfo);
                }

                var fullOrderDto = _mapper.Map<OrderTicket, FullOrderDto>(orderInfo);
                fullOrderDto.TicketDetails = ticketInfoList;
                fullOrderDtoList.Add(fullOrderDto);
            }

            _logger.LogInformation("Order history query completed successfully. Total orders: {OrderCount}", fullOrderDtoList.Count);

            return new Result<List<FullOrderDto>>()
            {
                Value = fullOrderDtoList
            };
        }
    }
}
