﻿using AutoMapper;
using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class TicketService : ITicketService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<TicketDto>>> GetFreeTickets(TicketSpecParam ticketsSpec)
        {
            var spec = new FreeTicketsInfoFilterByPrice(ticketsSpec);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

            if (!tickets.Any())
            {
                return ResultReturnService.CreateErrorResult<IReadOnlyList<TicketDto>>(ErrorStatusCode.NotFound, "No tickets found");
            }

            var ticketDto = _mapper.Map<IReadOnlyList<TicketDto>>(tickets);

            return new Result<IReadOnlyList<TicketDto>>()
            {
                Value = ticketDto
            };
        }

        public async Task<Result<IReadOnlyList<TicketDto>>> GetAllTickets(int concertId, bool isDescOredr)
        {
            var spec = new TicketsInfo(concertId, isDescOredr);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

            if (!tickets.Any())
            {
                return ResultReturnService.CreateErrorResult<IReadOnlyList<TicketDto>>(ErrorStatusCode.NotFound, "No tickets found");
            }

            var ticketDto = _mapper.Map<IReadOnlyList<TicketDto>>(tickets);

            return new Result<IReadOnlyList<TicketDto>>()
            {
                Value = ticketDto
            };
        }

        public async Task<Result> AddTicketsAsync(List<TicketAddDto> ticketsDto)
        {
            var tickets = _mapper.Map<List<Ticket>>(ticketsDto);
            foreach (var ticket in tickets)
            {
                await _unitOfWork.Repository<Ticket>().Add(ticket);
                var added = await _unitOfWork.Complete();
                if (added < 0)
                {
                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be added to db");
                }
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeleteTickets(List<int> ticketsIds, int concertId)
        {
            var spec = new TicketDelete(ticketsIds, concertId);
            var tickets = await _unitOfWork.Repository<Ticket>().ListAsync(spec);

            if (tickets == null)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "No tickets found");
            }

            foreach (var ticket in tickets)
            {
                _unitOfWork.Repository<Ticket>().Delete(ticket);
            }

            var deleted = await _unitOfWork.Complete();
            if (deleted < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be deletd from db");
            }

            return ResultReturnService.CreateSuccessResult();
        }
    }
}
