using AutoMapper;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketService> _logger;

        public TicketService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<TicketDto>>> GetFreeTicketsAsync(TicketSpecParam ticketsSpec)
        {
            var spec = new FreeTicketsInfoFilterByPrice(ticketsSpec);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

            if (!tickets.Any())
            {
                _logger.LogWarning("No free tickets found");

                return ResultReturnService.CreateErrorResult<IReadOnlyList<TicketDto>>(ErrorStatusCode.NotFound, "No tickets found");
            }

            var ticketDto = _mapper.Map<IReadOnlyList<TicketDto>>(tickets);
            _logger.LogInformation("Free tickets were successfully received. Count: {Count}", ticketDto.Count);

            return new Result<IReadOnlyList<TicketDto>>()
            {
                Value = ticketDto
            };
        }

        public async Task<Result<IReadOnlyList<TicketDto>>> GetAllTicketsAsync(int concertId, bool isDescOredr)
        {
            var spec = new TicketsInfo(concertId, isDescOredr);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

            if (!tickets.Any())
            {
                _logger.LogWarning("No tickets found for concertId {ConcertId}", concertId);

                return ResultReturnService.CreateErrorResult<IReadOnlyList<TicketDto>>(ErrorStatusCode.NotFound, "No tickets found");
            }

            var ticketDto = _mapper.Map<IReadOnlyList<TicketDto>>(tickets);
            _logger.LogInformation("All tickets were successfully received. Count: {Count}", ticketDto.Count);

            return new Result<IReadOnlyList<TicketDto>>()
            {
                Value = ticketDto
            };
        }

        public async Task<Result> AddTicketsAsync(List<TicketAddDto> ticketsDto)
        {
            var tickets = _mapper.Map<List<Ticket>>(ticketsDto);

            //TODO
            foreach (var ticket in tickets)
            {
                await _unitOfWork.Repository<Ticket>().AddAsync(ticket);
                var added = await _unitOfWork.CompleteAsync();
                if (added < 0)
                {
                    _logger.LogError("Failed to add tickets to the database");

                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be added to db");
                }
            }

            _logger.LogInformation("Tickets were successfully added to the database");

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeleteTicketsAsync(List<int> ticketsIds, int concertId)
        {
            var spec = new TicketDelete(ticketsIds, concertId);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

            if (!tickets.Any())
            {
                _logger.LogWarning("No tickets found for deletion");

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "No tickets found");
            }

            _unitOfWork.Repository<Ticket>().DeleteRange(tickets);
            var deleted = await _unitOfWork.CompleteAsync();

            if (deleted < 0)
            {
                _logger.LogError("Failed to delete tickets from the database");

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be deletd from db");
            }

            _logger.LogInformation("Tickets were successfully deleted from the database");

            return ResultReturnService.CreateSuccessResult();
        }
    }
}
