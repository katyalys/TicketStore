using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Services;
using Catalog.WebApi.Helpers;
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
            var addedPlace = await _placeService.AddPlaceAsync(place);

            return ErrorHandle.HandleResult(addedPlace);
        }

        [HttpDelete("DeletePlace")]
        public async Task<IActionResult> DeletePlace(int placeId)
        {
            var deletedPlace = await _placeService.DeletePlaceAsync(placeId);

            return ErrorHandle.HandleResult(deletedPlace);
        }

        [HttpPost("UpdatePlace")]
        public async Task<IActionResult> UpdatePlace(PlaceModel placeModel, int placeId)
        {
            var existingPlace = await _placeService.GetPlace(placeId);
            var place = _mapper.Map(placeModel, existingPlace.Value);
            var updatedPlace = await _placeService.UpdatePlaceAsync(place);

            return ErrorHandle.HandleResult(updatedPlace);
        }

        [HttpGet("GetPlace")]
        public async Task<IActionResult> GetPlace(int placeId)
        {
            var place = await _placeService.GetPlace(placeId);
            var placeModel = _mapper.Map<Result<PlaceModel>>(place);

            return ErrorHandle.HandleResult(placeModel);
        }

    }
}
