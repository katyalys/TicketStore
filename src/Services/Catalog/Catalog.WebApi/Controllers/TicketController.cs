using Catalog.Application.Dtos.TicketDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Specification.TicketsSpecifications;
using Catalog.WebApi.Helpers;
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
        public async Task<IActionResult> ListFreeTickets(TicketSpecParam ticketsSpec)
        {
            var freeTickets = await _ticketService.GetFreeTickets(ticketsSpec);

            return ErrorHandle.HandleResult(freeTickets);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListAllTickets")]
        public async Task<IActionResult> ListAllTickets(int concertId, bool isDescOredr)
        {
            var allTickets = await _ticketService.GetAllTickets(concertId, isDescOredr);

            return ErrorHandle.HandleResult(allTickets);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddTickets")]
        public async Task<IActionResult> AddTickets([FromBody] List<TicketAddDto> ticketsDto)
        {
            var result = await _ticketService.AddTicketsAsync(ticketsDto);

            return ErrorHandle.HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteTickets")]
        public async Task<IActionResult> DeleteTickets(List<int> ticketsIds, int concertId)
        {
            var result = await _ticketService.DeleteTickets(ticketsIds, concertId);

            return ErrorHandle.HandleResult(result);
        }
    }
}
