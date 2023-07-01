using AutoMapper;
using MediatR;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;
using Order.Domain.Entities;
using OrderClientGrpc;
using Order.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Enums;
using Order.Application.Dtos;

namespace Order.Application.Features.Orders.Queries.TicketsDetailedInfo
{
    public class TicketDetailedQueryHandler : IRequestHandler<TicketsDetailedQuery, List<TicketDetailInfo>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly OrderContext _orderContext;

        public TicketDetailedQueryHandler(IMapper mapper, IGenericRepository<OrderTicket> orderRepository, OrderContext orderContext)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _orderContext = orderContext;
        }

        public async Task<List<TicketDetailInfo>> Handle(TicketsDetailedQuery request, CancellationToken cancellationToken)
        {
            var ticketIds = _orderContext.Orders.Where(u => u.Id == request.OrderId 
                                                    && u.CustomerId == u.CustomerId 
                                                    && u.OrderStatus != Status.Canceled)
                                            .SelectMany(u => u.Tickets)
                                            .Where(t => t.TicketStatus == Status.Paid)
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
            return ticketInfoList;
        }
    }
}
