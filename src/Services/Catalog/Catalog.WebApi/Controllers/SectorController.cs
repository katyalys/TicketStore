using AutoMapper;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Application.Interfaces;
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
        private readonly IMapper _mapper;

        public SectorController(ISectorService sectorService, IMapper mapper)
        {
            _mapper = mapper;
            _sectorService = sectorService;
        }

        [HttpGet("ListAllSeats")]
        public async Task<List<SectorInfoDto>> ListAllSeats(int placeId)
        {
            var seats = await _sectorService.ListAllPossibleSeats(placeId);
            return seats;
        }

        [HttpGet("AddSector")]
        public async Task<IActionResult> AddSector(SectorFullInffoDto sectorAddDto)
        {
            await _sectorService.AddSector(sectorAddDto);
            return Ok();
        }

        [HttpDelete("DeleteSector")]
        public async Task<IActionResult> DeleteSector(int sectorId)
        {
            await _sectorService.DeleteSector(sectorId);
            return Ok();
        }

        [HttpPost("EditSector")]
        public async Task<IActionResult> EditSector(SectorFullInffoDto sectorFullInffoDto)
        {
            await _sectorService.UpdateSectorAsync(sectorFullInffoDto);
            return Ok();
        }


    }
}
