using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IMapper _mapper;

        public CatalogController(ICatalogService catalogService, IMapper mapper)
        {
            _mapper = mapper;
            _catalogService = catalogService;
        }

        [HttpGet("AllCurrentConcerts")]
        public async Task<IReadOnlyList<ConcertsShortViewModel>> GetAllConcerts([FromQuery] ConcertsSpecParam specParam, bool isDescOrder = false)
        {
            var concerts = await _catalogService.GetCurrentConcerts(specParam, isDescOrder);

            return _mapper.Map<IReadOnlyList<ConcertsShortViewModel>>(concerts);
        }

        [HttpGet("GetConcertById/{id}")]
        public async Task<FullInfoConcertModel> ConcertById(int id)
        {
            var concert = await _catalogService.GetConcert(id);
            return _mapper.Map<FullInfoConcertModel>(concert);
        }

        [HttpGet("SearchConcerts")]
        public async Task<IReadOnlyList<ConcertsShortViewModel>> SearchConcerts([FromQuery] string searchTerm)
        {
            var concerts = await _catalogService.GetSearchedConcerts(searchTerm);
            return _mapper.Map<IReadOnlyList<ConcertsShortViewModel>>(concerts);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task AddConcert(FullInfoConcertModel fullInfoConcertModel)
        {
            var concert = _mapper.Map<Concert>(fullInfoConcertModel);
            await _catalogService.AddConcertAsync(concert);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConcert(int id)
        {
            try
            {
                await _catalogService.DeleteConcertAsync(id);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("EditConcert")]
        public async Task<IActionResult> EditConcert1(FullInfoConcertModel concertFullInfo, int idConcert)
        {
            var existingConcert = await _catalogService.GetConcert(idConcert);
            var updatedConcert = _mapper.Map(concertFullInfo, existingConcert);
            await _catalogService.UpdateConcertAsync(updatedConcert);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ListAllConcerts")]
        public async Task<IReadOnlyList<ConcertsShortViewModel>> GetAllConcerts()
        {
            var concerts = await _catalogService.GetAllConcerts();

            return _mapper.Map<IReadOnlyList<ConcertsShortViewModel>>(concerts);
        }
    }
}
