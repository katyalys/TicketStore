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
        Task<Result> AddPlaceAsync(Place place);
        Task<Result> DeletePlaceAsync(int placeId);
        Task<Result> UpdatePlaceAsync(Place updatedPlace);
        Task<Result<Place>> GetPlace(int placeId);
    }
}
