using Catalog.Application.Dtos.SectorDtos;
using Catalog.Application.Interfaces;
using Catalog.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class SectorController : Controller
    {
        private readonly ISectorService _sectorService;

        public SectorController(ISectorService sectorService)
        {
            _sectorService = sectorService;
        }

        [HttpGet("ListAllSeats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListAllSeats(int placeId)
        {
            var seats = await _sectorService.ListAllPossibleSeatsAsync(placeId);

            return ErrorHandle.HandleResult(seats);
        }

        [HttpPost("AddSector")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddSector(SectorFullInffoDto sectorAddDto)
        {
            var result = await _sectorService.AddSectorAsync(sectorAddDto);

            return ErrorHandle.HandleResult(result);
        }

        [HttpDelete("DeleteSector")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSector(int sectorId)
        {
            var result = await _sectorService.DeleteSectorAsync(sectorId);

            return ErrorHandle.HandleResult(result);
        }

        [HttpPost("EditSector")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditSector(SectorFullInffoDto sectorFullInffoDto)
        {
            var result = await _sectorService.UpdateSectorAsync(sectorFullInffoDto);

            return ErrorHandle.HandleResult(result);
        }
    }
}
