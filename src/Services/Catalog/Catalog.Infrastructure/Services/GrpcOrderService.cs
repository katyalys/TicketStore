using AutoMapper;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Catalog.Infrastructure.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OrderServerGrpc;

namespace Catalog.Infrastructure.Services
{
    public class GrpcOrderService : OrderProtoService.OrderProtoServiceBase
    {
        private readonly IRedisRepository _redisRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GrpcOrderService(IRedisRepository redisRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _redisRepository = redisRepository;
            _unitOfWork = unitOfWork;
        }

        public override async Task<TicketOrderDto> GetTicketsToOrder(GetTicketsRequest ticketsRequest, ServerCallContext context)
        {
            var basket = await _redisRepository.Get<Basket>(ticketsRequest.UserId);

            if (basket == null)
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Product with ID={ticketsRequest.UserId} is not found."));
            }

            var ticketOrderModel = _mapper.Map<TicketOrderDto>(basket);
            ticketOrderModel.UserId = ticketsRequest.UserId;
            return ticketOrderModel;
        }

        public override async Task<TicketList> GetTicketInfo(GetTicketInfoRequest ticketRequest, ServerCallContext context)
        {
            var ticketList = new TicketList();
            foreach (var ticketId in ticketRequest.TicketId)
            {
                var spec = new TicketsInfo(ticketId);
                var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);

                if (ticket == null)
                {
                    throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Product with ID={ticket.Id} is not found."));
                }

                var ticketDto = _mapper.Map<TicketDto>(ticket);
                ticketDto.Concert.Date = _mapper.Map<Timestamp>(ticket.Concert.Date);
                ticketList.TicketDto.Add(ticketDto);
            }

            return ticketList;
        }

        public override async Task<TicketDateList> GetTicketDate(GetTicketDateRequest ticketRequest, ServerCallContext context)
        {

            var ticketList = new TicketDateList();
            foreach (var ticketId in ticketRequest.TicketId)
            {
                var spec = new TicketsInfo(ticketId);
                var ticket = await _unitOfWork.Repository<Ticket>().GetEntityWithSpec(spec);

                if (ticket == null)
                {
                    throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Product with ID={ticket.Id} is not found."));
                }

                var ticketDate = new TicketDate();
                DateTime concertDate = ticket.Concert.Date;
                DateTime utcConcertDate = concertDate.ToUniversalTime().AddHours(3);
                ticketDate.Date = Timestamp.FromDateTime(utcConcertDate);
                ticketDate.TicketId = ticketId;

                ticketList.TicketDate.Add(ticketDate);
            }

            return ticketList;
        }

    }
}
