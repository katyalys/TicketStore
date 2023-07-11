using AutoMapper;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.SectorsSpecifications;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services
{
    public class SectorService : ISectorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SectorService> _logger;

        public SectorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SectorService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<SectorInfoDto>>> ListAllPossibleSeatsAsync(int placeId)
        {
            var spec = new SectorsByPlaceSpec(placeId);
            var sectors = await _unitOfWork.Repository<Sector>().ListAsync(spec);

            if (!sectors.Any())
            {
                _logger.LogWarning("No sectors found for placeId {PlaceId}", placeId);

                return ResultReturnService.CreateErrorResult<List<SectorInfoDto>>(ErrorStatusCode.NotFound, "No sectors with such placeId");
            }

            var sectorsDto = _mapper.Map<IReadOnlyList<SectorInfoDto>>(sectors);
            var modifiedSectorsDto = new List<SectorInfoDto>();

            //TODO
            foreach (var sectorInfo in sectorsDto)
            {
                var seatRanges = new List<SeatRangeDto>();

                int row = 1;
                int startingSeat = 1;
                int endingSeat = sectorInfo.RowSeatNumber;

                while (endingSeat <= sectorInfo.RowSeatNumber * sectorInfo.RowNumber)
                {
                    var seatRange = new SeatRangeDto
                    {
                        Row = row,
                        SeatRange = $"{startingSeat} - {endingSeat}"
                    };
                    seatRanges.Add(seatRange);

                    row++;
                    startingSeat = endingSeat + 1;
                    endingSeat += sectorInfo.RowSeatNumber;
                }

                sectorInfo.SeatRanges = seatRanges;
                modifiedSectorsDto.Add(sectorInfo);
            }

            _logger.LogInformation("List of possible seats was successfully received. Count: {Count}", modifiedSectorsDto.Count);

            return new Result<List<SectorInfoDto>>()
            {
                Value = modifiedSectorsDto
            };
        }

        public async Task<Result> AddSectorAsync(SectorFullInffoDto sectorAddDto)
        {
            if (!Enum.IsDefined(typeof(SectorName), sectorAddDto.Name))
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Invalid sector name");
            }

            var place = await _unitOfWork.Repository<Place>().GetByIdAsync(sectorAddDto.PlaceId);

            if (place == null)
            {
                _logger.LogWarning("No place found with id {PlaceId} ", sectorAddDto.PlaceId);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "No place with such id");
            }

            SectorName enumName = (SectorName)Enum.Parse(typeof(SectorName), sectorAddDto.Name);
            var spec = new SectorsByPlaceSpec(sectorAddDto.PlaceId, enumName);
            var sector = await _unitOfWork.Repository<Sector>().GetEntityWithSpecAsync(spec);

            if (sector != null)
            {
                _logger.LogWarning("Sector already exists for place with id {PlaceId}", sectorAddDto.PlaceId);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Already place has such sector");
            }

            sectorAddDto.Price = TicketPriceCalculator.CalculatePrice(sectorAddDto.Price, enumName, place.City);
            var sectorsDto = _mapper.Map<Sector>(sectorAddDto);
            await _unitOfWork.Repository<Sector>().AddAsync(sectorsDto);
            var added = await _unitOfWork.CompleteAsync();

            if (added < 0)
            {
                _logger.LogError("Failed to add sector to the database");

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be added to db");
            }

            _logger.LogInformation("Sector with {Name} for place with id {PlaceId} was successfully added to database", 
                sectorAddDto.Name, sectorAddDto.PlaceId);

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeleteSectorAsync(int sectorId)
        {
            var sector = await _unitOfWork.Repository<Sector>().GetByIdAsync(sectorId);

            if (sector == null)
            {
                _logger.LogWarning("No sector found with id {SectorId}", sectorId);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "No sector with such id");
            }

            var spec = new SectorsToDeleteSpec(sectorId, sector.PlaceId);
            var sectorWithTickets = await _unitOfWork.Repository<Sector>().GetEntityWithSpecAsync(spec);

            if (sectorWithTickets != null)
            {
                _logger.LogWarning("Cannot delete sector with id {SectorId} because of existing tickets", sectorId);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Cant delete sector because of existing tickets");
            }

            _unitOfWork.Repository<Sector>().Delete(sector);
            var deleted = await _unitOfWork.CompleteAsync();

            if (deleted < 0)
            {
                _logger.LogError("Failed to delete sector with id {SectorId} from the database", sectorId);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be deletd from db");
            }

            _logger.LogInformation("Sector with id {SectorId} was successfully deleted from database", sectorId);

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> UpdateSectorAsync(SectorFullInffoDto sectorFullInffoDto)
        {
            if (!Enum.IsDefined(typeof(SectorName), sectorFullInffoDto.Name))
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Invalid sector name");
            }

            var enumName = (SectorName)Enum.Parse(typeof(SectorName), sectorFullInffoDto.Name);
            var spec = new SectorsByPlaceSpec(sectorFullInffoDto.PlaceId, enumName);
            var sector = await _unitOfWork.Repository<Sector>().GetEntityWithSpecAsync(spec);

            if (sector == null)
            {
                _logger.LogWarning("No sector found with placeId {PlaceId}", sectorFullInffoDto.PlaceId);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.NotFound, "No sector with such placeId");
            }

            var ticketSpec = new TicketsSoldForSectorSpec(sector.Id);
            var ticketsSold = await _unitOfWork.Repository<Ticket>().ListAsync(ticketSpec);

            if (ticketsSold.Any())
            {
                var maxRowNumber = ticketsSold.Max(t => t.Row);
                var maxSeatsInRow = ticketsSold.Max(t => t.Seat);

                if (sectorFullInffoDto.RowNumber <= maxRowNumber || sectorFullInffoDto.RowSeatNumber <= maxSeatsInRow)
                {
                    _logger.LogWarning("Cannot decrease the number of rows or seats for sector with id {SectorId}. Tickets have already been sold.", 
                        sector.Id);

                    return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction,
                        "Cannot decrease the number of rows or seats. Tickets have already been sold.");
                }
            }

            var updatedSector = _mapper.Map(sectorFullInffoDto, sector);
            _unitOfWork.Repository<Sector>().Update(updatedSector);
            var updated = await _unitOfWork.CompleteAsync();

            if (updated < 0)
            {
                _logger.LogError("Failed to update sector with id {SectorId} in the database", sector.Id);

                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be updated in db");
            }

            _logger.LogInformation("Sector with id {SectorId} was successfully updated in the database", sector.Id);

            return ResultReturnService.CreateSuccessResult();
        }
    }
}
