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
        private readonly IMapper _mapper;

        public CatalogController(ICatalogService catalogService, IMapper mapper)
        {
            _mapper = mapper;
            _catalogService = catalogService;
        }

        [HttpGet("GetCurrentConcerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentConcerts([FromQuery] ConcertsSpecParam specParam, bool isDescOrder = false)
        {
            var concerts = await _catalogService.GetCurrentConcerts(specParam, isDescOrder);
            var mappedConcerts = _mapper.Map<Result<IReadOnlyList<ConcertsShortViewModel>>>(concerts);

            return ErrorHandle.HandleResult(mappedConcerts);
        }

        [HttpGet("GetConcertById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConcertById(int id)
        {
            var concert = await _catalogService.GetConcert(id);
            var mappedConcert = _mapper.Map<Result<FullInfoConcertModel>>(concert);

            return ErrorHandle.HandleResult(mappedConcert);
        }

        [HttpGet("SearchConcerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchConcerts([FromQuery] string searchTerm)
        {
            var concerts = await _catalogService.GetSearchedConcerts(searchTerm);
            var mappedConcerts = _mapper.Map<Result<IReadOnlyList<ConcertsShortViewModel>>>(concerts);

            return ErrorHandle.HandleResult(mappedConcerts);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddConcert")]
        public async Task<IActionResult> AddConcert(FullInfoConcertModel fullInfoConcertModel)
        {
            var concert = _mapper.Map<Concert>(fullInfoConcertModel);
            var result = await _catalogService.AddConcertAsync(concert);

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
        public async Task<IActionResult> EditConcert(FullInfoConcertModel concertFullInfo, int idConcert)
        {
            var existingConcert = await _catalogService.GetConcert(idConcert);
            var updatedConcert = _mapper.Map(concertFullInfo, existingConcert.Value);
            var result = await _catalogService.UpdateConcertAsync(updatedConcert);

            return ErrorHandle.HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListAllConcerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllConcerts()
        {
            var concerts = await _catalogService.GetAllConcerts();
            var mappedConcerts = _mapper.Map<Result<IReadOnlyList<ConcertsShortViewModel>>>(concerts);

            return ErrorHandle.HandleResult(mappedConcerts);
        }
    }
}
