using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification;
using Catalog.Domain.Specification.ConcertSpecifications;
using Catalog.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet("GetCurrentConcerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentConcerts([FromQuery] ConcertsSpecParam specParam, bool isDescOrder = false)
        {
            var concerts = await _catalogService.GetCurrentConcerts(specParam, isDescOrder);

            return ErrorHandle.HandleResult(concerts);
        }

        [HttpGet("GetConcertById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConcertById(int id)
        {
            var concert = await _catalogService.GetConcert(id);

            return ErrorHandle.HandleResult(concert);
        }

        [HttpGet("SearchConcerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchConcerts([FromQuery] string searchTerm)
        {
            var concerts = await _catalogService.GetSearchedConcerts(searchTerm);

            return ErrorHandle.HandleResult(concerts);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddConcert")]
        public async Task<IActionResult> AddConcert(FullInfoConcertDto fullInfoConcertModel)
        {
            var result = await _catalogService.AddConcertAsync(fullInfoConcertModel);

            return ErrorHandle.HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteConcert/{id}")]
        public async Task<IActionResult> DeleteConcert(int id)
        {
            var result = await _catalogService.DeleteConcertAsync(id);

            return ErrorHandle.HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("EditConcert")]
        public async Task<IActionResult> EditConcert(FullInfoConcertDto concertFullInfo, int idConcert)
        {
            var result = await _catalogService.UpdateConcertAsync(concertFullInfo, idConcert);

            return ErrorHandle.HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListAllConcerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllConcerts()
        {
            var concerts = await _catalogService.GetAllConcerts();

            return ErrorHandle.HandleResult(concerts);
        }
    }
}
