using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.PlacesSpecifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class PlaceService : IPlaceService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlaceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddPlaceAsync(Place place)
        {
            var places = await _unitOfWork.Repository<Place>().ListAllAsync();
            bool placeExists = places.Any(p => p.City == place.City && p.Street == place.Street && p.PlaceNumber == place.PlaceNumber);

            if (placeExists)
            {
                throw new Exception("Place already exists");
            }
            await _unitOfWork.Repository<Place>().Add(place);
            await _unitOfWork.Complete();
        }

        public async Task DeletePlaceAsync(int placeId)
        {
            var place = await _unitOfWork.Repository<Place>().GetByIdAsync(placeId);
            if (place == null)
            {
                throw new Exception("No place with such id");
            }

            var placesWithConcerts = new PlaceSpec(place.Id);
            var places = await _unitOfWork.Repository<Place>().ListAsync(placesWithConcerts);
            if (places.Any())
            {
                throw new Exception("Cant delete because of exsisting concerts");
            }

            foreach (var sector in place.Sectors)
            {
                _unitOfWork.Repository<Sector>().Delete(sector);
               // await _unitOfWork.Complete();
            }

            _unitOfWork.Repository<Place>().Delete(place);
            await _unitOfWork.Complete();
        }

        public async Task UpdatePlaceAsync(Place updatedPlace)
        {
            if (updatedPlace == null && updatedPlace.IsDeleted != true)
            {
                throw new Exception("No place with such id");
            }

            _unitOfWork.Repository<Place>().Update(updatedPlace);
            await _unitOfWork.Complete();
        }

        public Task<Place> GetPlace(int placeId)
        {
            var place = _unitOfWork.Repository<Place>().GetByIdAsync(placeId);

            return place;
        }
    }
}
