using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Dtos.PlaceDtos;
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

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpPost("AddPlace")]
        public async Task<IActionResult> AddPlace(PlaceDto placeDto)
        {
            var addedPlace = await _placeService.AddPlaceAsync(placeDto);

            return ErrorHandle.HandleResult(addedPlace);
        }

        [HttpDelete("DeletePlace")]
        public async Task<IActionResult> DeletePlace(int placeId)
        {
            var deletedPlace = await _placeService.DeletePlaceAsync(placeId);

            return ErrorHandle.HandleResult(deletedPlace);
        }

        [HttpPost("UpdatePlace")]
        public async Task<IActionResult> UpdatePlace(PlaceDto placeDto, int placeId)
        {
            var updatedPlace = await _placeService.UpdatePlaceAsync(placeDto, placeId);

            return ErrorHandle.HandleResult(updatedPlace);
        }

        [HttpGet("GetPlace")]
        public async Task<IActionResult> GetPlace(int placeId)
        {
            var place = await _placeService.GetPlace(placeId);

            return ErrorHandle.HandleResult(place);
        }

    }
}
