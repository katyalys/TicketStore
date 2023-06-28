using AutoMapper;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Data;
using Grpc.Core;
using OrderGrpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class GrpcOrderService : OrderProtoService.OrderProtoServiceBase
    {
        private readonly IRedisRepository _redisRepository;
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GrpcOrderService(IRedisRepository redisRepository, IMapper mapper, IUnitOfWork unitOfWork, CatalogContext context)
        {
            _mapper = mapper;
            _redisRepository = redisRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public override async Task<TicketOrderDto> GetTicketsToOrder(GetTicketsRequest ticketsRequest, ServerCallContext context)
        {
            var basket = await _redisRepository.Get<Basket>(ticketsRequest.UserId);

            if (basket == null)
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Product with ID={ticketsRequest.UserId} is not found."));
            }

            var ticketOrderModel = _mapper.Map<TicketOrderDto>(basket);
            return ticketOrderModel;
        }
    }
}
