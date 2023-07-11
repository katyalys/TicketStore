using AutoMapper;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OrderServerGrpc;

namespace Catalog.Infrastructure.Services
{
    public class GrpcOrderService : OrderProtoService.OrderProtoServiceBase
    {
        private readonly IRedisRepository _redisRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GrpcOrderService> _logger;

        public GrpcOrderService(IRedisRepository redisRepository, IMapper mapper, 
            IUnitOfWork unitOfWork, ILogger<GrpcOrderService> logger)
        {
            _mapper = mapper;
            _redisRepository = redisRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public override async Task<TicketOrderDto> GetTicketsToOrder(GetTicketsRequest ticketsRequest, ServerCallContext context)
        {
            var basket = await _redisRepository.GetAsync<Basket>(ticketsRequest.UserId);

            if (basket == null)
            {
                _logger.LogError("Basket not found for user ID {UserId}", ticketsRequest.UserId);
                throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Product with ID={ticketsRequest.UserId} is not found."));
            }

            var ticketOrderModel = _mapper.Map<TicketOrderDto>(basket);
            _logger.LogInformation("Tickets to order were successfully received.");

            return ticketOrderModel;
        }

        public override async Task<TicketList> GetTicketInfo(GetTicketInfoRequest ticketRequest, ServerCallContext context)
        {
            var ticketList = new TicketList();

            foreach (var ticketId in ticketRequest.TicketId)
            {
                var spec = new TicketsInfo(ticketId);
                var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpecAsync(spec);

                if (ticket == null)
                {
                    _logger.LogError("Ticket not found for ID {TicketId}", ticketId);
                    throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Product with ID={ticket.Id} is not found."));
                }

                var ticketDto = _mapper.Map<TicketDto>(ticket);
                ticketDto.Concert.Date = _mapper.Map<Timestamp>(ticket.Concert.Date);
                ticketList.TicketDto.Add(ticketDto);
            }

            _logger.LogInformation("Ticket information was successfully received. Count: {Count}", ticketList.TicketDto.Count);

            return ticketList;
        }

        public override async Task<TicketDateList> GetTicketDate(GetTicketDateRequest ticketRequest, ServerCallContext context)
        {
            var ticketList = new TicketDateList();

            foreach (var ticketId in ticketRequest.TicketId)
            {
                var spec = new TicketsInfo(ticketId);
                var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpecAsync(spec);

                if (ticket == null)
                {
                    _logger.LogError("Ticket not found for ID {TicketId}", ticketId);
                    throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Product with ID={ticket.Id} is not found."));
                }

                var ticketDate = new TicketDate();
                var concertDate = ticket.Concert.Date;
                var utcConcertDate = concertDate.ToUniversalTime().AddHours(3);
                ticketDate.Date = Timestamp.FromDateTime(utcConcertDate);
                ticketDate.TicketId = ticketId;

                ticketList.TicketDate.Add(ticketDate);
            }

            _logger.LogInformation("Ticket dates were successfully received. Count: {Count}", ticketList.TicketDate.Count);

            return ticketList;
        }
    }
}
