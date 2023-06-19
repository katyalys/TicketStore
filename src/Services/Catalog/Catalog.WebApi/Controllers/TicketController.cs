using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("ListFreeTickets")]
        public async Task<ActionResult<IReadOnlyList<TicketDto>?>> ListFreeTickets(TicketSpecParam ticketsSpec)
        {
            var freeTickets = await _ticketService.GetFreeTickets(ticketsSpec);

            return Ok(freeTickets);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListAllTickets")]
        public async Task<ActionResult<IReadOnlyList<TicketDto>?>> ListAllTickets(int concertId, bool isDescOredr)
        {
            var allTickets = await _ticketService.GetAllTickets(concertId, isDescOredr);

            return Ok(allTickets);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AddTickets")]
        public async Task<ActionResult<IReadOnlyList<TicketDto>?>> AddTickets(List<TicketAddDto> ticketsDto)
        {
            await _ticketService.AddTicketsAsync(ticketsDto);

            return Ok();
        }
    }
}
