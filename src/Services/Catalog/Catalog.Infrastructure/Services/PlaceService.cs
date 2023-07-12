using AutoMapper;
using Catalog.Application.Dtos.PlaceDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.PlacesSpecifications;
using Catalog.Domain.Specification.SectorsSpecifications;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PlaceService> _logger;

        public PlaceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PlaceService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> AddPlaceAsync(PlaceDto placeDto)
        {
            var place = _mapper.Map<Place>(placeDto);
            var spec = new PlaceSpec(place);
            var placeExists = await _unitOfWork.Repository<Place>().GetEntityWithSpecAsync(spec);

            if (placeExists != null)
            {
                _logger.LogWarning("Place already exists");

                return ResultReturnService.CreateErrorResult<Result>
                    (ErrorStatusCode.WrongAction, "Place already exists");
            }

            await _unitOfWork.Repository<Place>().AddAsync(place);
            var result = await _unitOfWork.CompleteAsync();

            if (result < 0)
            {
                _logger.LogError("Failed to add place to the database");

                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be added to db");
            }

            _logger.LogInformation("Place was successfully added to database");

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeletePlaceAsync(int placeId)
        {
            var place = await _unitOfWork.Repository<Place>().GetByIdAsync(placeId);

            if (place == null)
            {
                _logger.LogWarning("No place found with id {PlaceId}", placeId);

                return ResultReturnService.CreateErrorResult<Result>
                    (ErrorStatusCode.NotFound, "No place with such id");
            }

            var placesWithConcerts = new PlaceSpec(place.Id);
            var places = await _unitOfWork.Repository<Place>().GetEntityWithSpecAsync(placesWithConcerts);

            if (places != null)
            {
                _logger.LogWarning("Cannot delete place with id {PlaceId} because of existing concerts", placeId);

                return ResultReturnService.CreateErrorResult<Result>
                    (ErrorStatusCode.WrongAction, "Cant delete because of exisiting concerts");
            }

            var placeToDelete = new SectorsByPlaceSpec(place.Id);
            var sectorsToDelete = await _unitOfWork.Repository<Sector>().ListAsync(placeToDelete);

            if (sectorsToDelete.Any())
            {
                _unitOfWork.Repository<Sector>().DeleteRange(sectorsToDelete);
            }

            _unitOfWork.Repository<Place>().Delete(place);
            var result = await _unitOfWork.CompleteAsync();

            if (result < 0)
            {
                _logger.LogError("Failed to delete place with id {PlaceId} from the database", placeId);

                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be deleted from db");
            }

            _logger.LogInformation("Place with id {PlaceId} was successfully deleted from database", placeId);

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> UpdatePlaceAsync(PlaceDto placeDto, int placeId)
        {
            var place = await _unitOfWork.Repository<Place>().GetByIdAsync(placeId);
            var updatedPlace = _mapper.Map(placeDto, place);

            if (updatedPlace == null || updatedPlace.IsDeleted == true)
            {
                _logger.LogWarning("Cannot update place with id {PlaceId} because place is deleted", placeId);

                return ResultReturnService.CreateErrorResult<Result>
                    (ErrorStatusCode.WrongAction, "Cant update because place is deleted");
            }

            _unitOfWork.Repository<Place>().Update(updatedPlace);
            var result = await _unitOfWork.CompleteAsync();

            if (result < 0)
            {
                _logger.LogError("Failed to update place with id {PlaceId} in the database", placeId);

                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be deleted from db");
            }

            _logger.LogInformation("Place with id {PlaceId} was successfully updated in the database", placeId);

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result<PlaceDto>> GetPlaceAsync(int placeId)
        {
            var place = await _unitOfWork.Repository<Place>().GetByIdAsync(placeId);

            if (place == null)
            {
                _logger.LogWarning("No place found with id {PlaceId}", placeId);

                return ResultReturnService.CreateErrorResult<PlaceDto>
                    (ErrorStatusCode.NotFound, "No place with such id");
            }

            var placeModel = _mapper.Map<PlaceDto>(place);
            _logger.LogInformation("Place with id {PlaceId} was successfully recieved", placeId);

            return new Result<PlaceDto>()
            {
                Value = placeModel,
            };
        }
    }
}
