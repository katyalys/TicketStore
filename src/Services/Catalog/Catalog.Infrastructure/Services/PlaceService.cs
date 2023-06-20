using AutoMapper;
using Catalog.Application.Dtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
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

        public PlaceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> AddPlaceAsync(Place place)
        {
            var spec = new PlaceSpec(place); 
            var placeExists = await _unitOfWork.Repository<Place>().GetEntityWithSpec(spec);
            if (placeExists != null)
            {
                return ResultReturnService.CreateErrorResult<Result>(ErrorStatusCode.WrongAction, "Place already exists");
            }

            await _unitOfWork.Repository<Place>().Add(place);
            var result = await _unitOfWork.Complete();

            if (result < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be added to db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeletePlaceAsync(int placeId)
        {
            var place = await _unitOfWork.Repository<Place>().GetByIdAsync(placeId);
            if (place == null)
            {
                return ResultReturnService.CreateErrorResult<Result>(ErrorStatusCode.NotFound, "No place with such id");
            }

            var placesWithConcerts = new PlaceSpec(place.Id);
            var places = await _unitOfWork.Repository<Place>().GetEntityWithSpec(placesWithConcerts);
            if (places != null)
            {
                return ResultReturnService.CreateErrorResult<Result>(ErrorStatusCode.WrongAction, "Cant delete because of exisiting concerts");
            }

            foreach (var sector in place.Sectors)
            {
                _unitOfWork.Repository<Sector>().Delete(sector);
            }

            _unitOfWork.Repository<Place>().Delete(place);
            var result = await _unitOfWork.Complete();

            if (result < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be deleted from db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> UpdatePlaceAsync(Place updatedPlace)
        {
            if (updatedPlace == null || updatedPlace.IsDeleted == true)
            {
                return ResultReturnService.CreateErrorResult<Result>(ErrorStatusCode.WrongAction, "Cant update because place is deleted");
            }

            _unitOfWork.Repository<Place>().Update(updatedPlace);
            var result = await _unitOfWork.Complete();
            if (result < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be deleted from db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result<Place>> GetPlace(int placeId)
        {
            var place = await _unitOfWork.Repository<Place>().GetByIdAsync(placeId);
            if (place == null)
            {
                return ResultReturnService.CreateErrorResult<Place>(ErrorStatusCode.NotFound, "No place with such id");
            }

            return new Result<Place>()
            {
                Value = place
            };
        }
    }
}
