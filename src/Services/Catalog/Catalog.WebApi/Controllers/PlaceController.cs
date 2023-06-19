using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class PlaceController : Controller
    {
        private readonly IPlaceService _placeService;
        private readonly IMapper _mapper;

        public PlaceController(IPlaceService placeService, IMapper mapper)
        {
            _mapper = mapper;
            _placeService = placeService;
        }

        [HttpPost("AddPlace")]
        public async Task<IActionResult> AddPlace(PlaceModel placeModel)
        {
            var place = _mapper.Map<Place>(placeModel);
            await _placeService.AddPlaceAsync(place);

            return Ok();
        }

        [HttpDelete("DeletePlace")]
        public async Task<IActionResult> DeletePlace(int placeId)
        {
            await _placeService.DeletePlaceAsync(placeId);

            return Ok();
        }

        [HttpPost("UpdatePlace")]
        public async Task<IActionResult> UpdatePlace(PlaceModel placeModel, int placeId)
        {
            var existingPlace = await _placeService.GetPlace(placeId);
            var updatedConcert = _mapper.Map(placeModel, existingPlace);
            await _placeService.UpdatePlaceAsync(updatedConcert);

            return Ok();
        }

    }
}
