using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface IPlaceService
    {
        Task AddPlaceAsync(Place place);
        Task DeletePlaceAsync(int placeId);
        Task UpdatePlaceAsync(Place updatedPlace);
        Task<Place> GetPlace(int placeId);
    }
}
