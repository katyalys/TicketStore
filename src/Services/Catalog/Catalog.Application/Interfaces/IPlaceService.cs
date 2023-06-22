using Catalog.Application.Dtos.PlaceDtos;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface IPlaceService
    {
        Task<Result> AddPlaceAsync(PlaceDto placeDto);
        Task<Result> DeletePlaceAsync(int placeId);
        Task<Result> UpdatePlaceAsync(PlaceDto placeDto, int placeId);
        Task<Result<PlaceDto>> GetPlace(int placeId);
    }
}
