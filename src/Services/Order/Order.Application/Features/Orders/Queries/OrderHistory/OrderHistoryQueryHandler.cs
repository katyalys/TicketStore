using AutoMapper;
using Grpc.Net.Client;
using MediatR;
using Order.Application.Dtos;
using Order.Domain.Entities;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
using OrderClientGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries.OrderHistory
{
    public class OrderHistoryQueryHandler : IRequestHandler<OrderHistoryQuery, List<FullOrderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly OrderContext _orderContext;

        public OrderHistoryQueryHandler(IMapper mapper, IGenericRepository<OrderTicket> orderRepository, OrderContext orderContext)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderContext = orderContext;
        }

        public async Task<List<FullOrderDto>> Handle(OrderHistoryQuery request, CancellationToken cancellationToken)
        {
            var fullOrderDtoList = new List<FullOrderDto>();
            var orderInfoList = _orderContext.Orders.Where(u => u.CustomerId == request.CustomerId);
            foreach (var orderInfo in orderInfoList)
            {
                var ticketIds = orderInfoList.SelectMany(u => u.Tickets)
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
                var ticketOrderDto = await client.GetTicketInfoAsync(grpcRequest);

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

            return fullOrderDtoList;
        }
    }
}
