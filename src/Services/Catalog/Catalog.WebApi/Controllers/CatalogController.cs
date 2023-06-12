using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IMapper _mapper;

        public CatalogController(ICatalogService catalogService, IMapper mapper)
        {
            _mapper = mapper;
            _catalogService = catalogService;
        }

        [HttpGet("AllConcerts")]
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

        [HttpPost("Add")]
        public async Task AddConcert(FullInfoConcertModel fullInfoConcertModel)
        {
            var concert = _mapper.Map<Concert>(fullInfoConcertModel);
            await _catalogService.AddConcertAsync(concert);
        }

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

        [HttpPost("EditConcert1")]
        public async Task<IActionResult> EditConcert1(FullInfoConcertModel concertFullInfo, int idConcert)
        {
            var existingConcert = await _catalogService.GetConcert(idConcert);

            //Perform mapping of updated information
            var updatedConcert = _mapper.Map(concertFullInfo, existingConcert);

            //Update the concert in the catalog
            await _catalogService.UpdateConcertAsync(updatedConcert);
            return Ok();
        }

        [HttpGet("Admin/ListAllConcerts")]
        public async Task<IReadOnlyList<ConcertsShortViewModel>> GetAllConcerts()
        {
            var concerts = await _catalogService.GetAllConcerts();

            return _mapper.Map<IReadOnlyList<ConcertsShortViewModel>>(concerts);
        }
    }
}
