using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Order.Application.Dtos;
using Order.Domain.Entities;
using Order.Domain.Interfaces;
using Order.Domain.Specification;
using Order.Infrastructure.Data;
using OrderClientGrpc;
using System.Linq;

namespace Order.Application.Features.Orders.Queries.AllOrders
{
    public class AllOrdersQueryHandler : IRequestHandler<AllOrdersQuery, List<OrderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly OrderContext _orderContext;

        public AllOrdersQueryHandler(IMapper mapper, IGenericRepository<OrderTicket> orderRepository, OrderContext orderContext)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderContext = orderContext;
        }

        public async Task<List<OrderDto>> Handle(AllOrdersQuery request, CancellationToken cancellationToken)
        {
            var fullOrderDtoList = new List<OrderDto>();

            GetTicketsSpec getTicketSpec = new GetTicketsSpec();
            var orderInfoList = await _orderRepository.ListAsync(getTicketSpec);

            var orderGroups = orderInfoList.GroupBy(u => u.CustomerId);

            foreach (var orderGroup in orderGroups)
            {
                var customerId = orderGroup.Key; 
                var ticketIds = orderGroup.SelectMany(u => u.Tickets)
                                         .Select(t => t.TicketBasketId)
                                         .ToList();

                if (ticketIds == null)
                {
                    throw new Exception("Check input data");
                }

                var grpcRequest = new GetTicketInfoRequest();
                grpcRequest.TicketId.Add(ticketIds);

                using var channel = GrpcChannel.ForAddress("https://localhost:5046");
                var client = new OrderProtoService.OrderProtoServiceClient(channel);
                var ticketOrderDto = client.GetTicketInfo(grpcRequest);

                List<TicketDetailInfo> ticketInfoList = new List<TicketDetailInfo>();
                foreach (var ticketDto in ticketOrderDto.TicketDto)
                {
                    var ticketInfo = _mapper.Map<TicketDetailInfo>(ticketDto);
                    ticketInfo.Date = _mapper.Map<DateTime>(ticketDto.Concert.Date);
                    ticketInfoList.Add(ticketInfo);
                }

                var fullOrderDto = _mapper.Map<OrderDto>(orderGroup.FirstOrDefault());
                fullOrderDto.CustomerId = customerId;
                fullOrderDto.TicketDetails = ticketInfoList;
                fullOrderDtoList.Add(fullOrderDto);
            }

            return fullOrderDtoList;
        }
    }
}
