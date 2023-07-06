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

namespace Order.Application.Features.Orders.Queries.OrderHistory
{
    public class OrderHistoryQueryHandler : IRequestHandler<OrderHistoryQuery, Result<List<FullOrderDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly string _url;
        private readonly GrpcChannel _channel;
        private readonly OrderProtoService.OrderProtoServiceClient _client;

        public OrderHistoryQueryHandler(IMapper mapper, IConfiguration configuration,
                        IGenericRepository<OrderTicket> orderRepository)
        {
            _mapper = mapper;
            _url = configuration["GrpcServer:Address"];
            _orderRepository = orderRepository;
            _channel = GrpcChannel.ForAddress(_url);
            _client = new OrderProtoService.OrderProtoServiceClient(_channel);
        }

        public async Task<Result<List<FullOrderDto>>> Handle(OrderHistoryQuery request, CancellationToken cancellationToken)
        {
            var fullOrderDtoList = new List<FullOrderDto>();
            var spec = new OrderByCustomerSpec(request.CustomerId);
            var orderInfoList = await _orderRepository.ListAsync(spec);

            if (!orderInfoList.Any())
            {
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

            return new Result<List<FullOrderDto>>()
            {
                Value = fullOrderDtoList
            };
        }
    }
}
