﻿using AutoMapper;
using MediatR;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;
using OrderClientGrpc;
using Order.Domain.Enums;
using Order.Application.Dtos;
using Order.Domain.ErrorModels;
using Order.Infrastructure.Services;
using Order.Domain.Entities;
using Order.Domain.Interfaces;
using Order.Domain.Specification.OrderSpecifications;
using Microsoft.Extensions.Logging;

namespace Order.Application.Features.Orders.Queries.TicketsDetailedInfo
{
    public class TicketDetailedQueryHandler : IRequestHandler<TicketsDetailedQuery, Result<List<TicketDetailInfoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly OrderProtoService.OrderProtoServiceClient _client;
        private readonly IGenericRepository<OrderTicket> _orderRepository;
        private readonly ILogger<TicketDetailedQueryHandler> _logger;

        public TicketDetailedQueryHandler(IMapper mapper,
             OrderProtoService.OrderProtoServiceClient client,
             IGenericRepository<OrderTicket> orderRepository,
             ILogger<TicketDetailedQueryHandler> logger)
        {
            _mapper = mapper;
            _client = client;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<List<TicketDetailInfoDto>>> Handle(TicketsDetailedQuery request, CancellationToken cancellationToken)
        {
            var spec = new OrderWithTicketsSpec(request.OrderId, request.CustomerId);
            var tickets = await _orderRepository.ListAsync(spec);
            var ticketIds = tickets.SelectMany(u => u.Tickets.Where(t => t.TicketStatus == Status.Paid))
                                       .Select(t => t.TicketBasketId);

            if (!ticketIds.Any())
            {
                _logger.LogWarning("No tickets found for OrderId {OrderId} and CustomerId {CustomerId}", 
                    request.OrderId, request.CustomerId);

                return ResultReturnService.CreateErrorResult<List<TicketDetailInfoDto>>(ErrorStatusCode.NotFound,
                    "No tickets were found");
            }

            var grpcRequest = new GetTicketInfoRequest();
            grpcRequest.TicketId.Add(ticketIds);
            var ticketOrderDto = await _client.GetTicketInfoAsync(grpcRequest);

            if (ticketOrderDto == null || !ticketOrderDto.TicketDto.Any())
            {
                _logger.LogWarning("No tickets found for OrderId {OrderId} and CustomerId {CustomerId}", 
                    request.OrderId, request.CustomerId);

                return ResultReturnService.CreateErrorResult<List<TicketDetailInfoDto>>(ErrorStatusCode.NotFound,
                    "No tickets were found");
            }

            var ticketInfoList = new List<TicketDetailInfoDto>();

            foreach (var ticketDto in ticketOrderDto.TicketDto)
            {
                var ticketInfo = _mapper.Map<TicketDetailInfoDto>(ticketDto);
                ticketInfo.Date = _mapper.Map<DateTime>(ticketDto.Concert.Date);
                ticketInfoList.Add(ticketInfo);
            }

            _logger.LogInformation("Ticket detailed query completed successfully. Total tickets: {TicketCount}", 
                ticketInfoList.Count);

            return new Result<List<TicketDetailInfoDto>>()
            {
                Value = ticketInfoList
            };
        }
    }
}
