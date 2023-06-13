using AutoMapper;
using Catalog.Application.Dtos.SectorDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.SectorsSpecifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class SectorService : ISectorService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SectorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SectorInfoDto>> ListAllPossibleSeats(int placeId)
        {
            var spec = new SectorsByPlaceSpec(placeId);
            var sectors = await _unitOfWork.Repository<Sector>().ListAsync(spec);

            var sectorsDto = _mapper.Map<IReadOnlyList<SectorInfoDto>>(sectors);

            var modifiedSectorsDto = new List<SectorInfoDto>();

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

            return modifiedSectorsDto;
        }

        public async Task AddSector(SectorFullInffoDto sectorAddDto)
        {
            // Validate if the entered sector name is valid
            if (!Enum.IsDefined(typeof(SectorName), sectorAddDto.Name))
            {
                throw new Exception("Invalid sector name");
            }

            var place = await _unitOfWork.Repository<Place>().GetByIdAsync(sectorAddDto.PlaceId);
            if (place == null)
            {
                throw new Exception("No place with such Id");
            }

            SectorName enumName = (SectorName)Enum.Parse(typeof(SectorName), sectorAddDto.Name);
            var spec = new SectorsByPlaceSpec(sectorAddDto.PlaceId, enumName);
            var sector = await _unitOfWork.Repository<Sector>().GetEntityWithSpec(spec);
            if (sector != null)
            {
                throw new Exception("Already place has such sector");
            }

            sectorAddDto.Price = TicketPriceCalculator.CalculatePrice(sectorAddDto.Price, enumName, place.City);
            var sectorsDto = _mapper.Map<Sector>(sectorAddDto);
            await _unitOfWork.Repository<Sector>().Add(sectorsDto);
            await _unitOfWork.Complete();
        }

        //
        public async Task DeleteSector(int sectorId, int placeId)
        {
            //var place = await _unitOfWork.Repository<Place>().GetByIdAsync(placeId);
            //if (place == null)
            //{
            //    throw new Exception("No place with such Id");
            //}

            var spec = new SectorsToDeleteSpec(placeId, sectorId);
            var sector = await _unitOfWork.Repository<Sector>().GetEntityWithSpec(spec);
            if (sector != null)
            {
                throw new Exception("Cant delete sector because of existing tickets");
            }
            _unitOfWork.Repository<Sector>().Delete(sector);
            await _unitOfWork.Complete();
        }

        public async Task UpdateSectorAsync(SectorFullInffoDto sectorFullInffoDto)
        {
            //var spec = new SectorsByPlaceSpec(sectorFullInffoDto.Name, sectorFullInffoDto.PlaceId);
            //var sector = await _unitOfWork.Repository<Sector>().GetEntityWithSpec(spec);
            //if (sector == null)
            //{
            //    throw new Exception("No sector with such placeId or name");
            //}

            //var ticketSpec = new TicketsSoldForSectorSpec(sector.Id, (int)StatusTypes.Bought);
            //var ticketsSold = await _unitOfWork.Repository<Ticket>().ListAsync(ticketSpec);

            //if (ticketsSold.Any())
            //{
            //    // Retrieve the maximum row number and maximum seats in a row from the existing tickets
            //    var maxRowNumber = ticketsSold.Max(t => t.Row);
            //    var maxSeatsInRow = ticketsSold.Max(t => t.Seat);

            //    // Calculate the maximum seat number in the last row
            //    //var maxSeatNumber = (maxRowNumber - 1) * maxSeatsInRow + sectorFullInffoDto.RowSeatNumber;
            //    // Check if the number of rows or seats in each row is being decreased
            //    if (sectorFullInffoDto.RowNumber < maxRowNumber || sectorFullInffoDto.RowSeatNumber < maxSeatsInRow 
            //        /*|| sectorFullInffoDto.RowSeatNumber < maxSeatNumber*/)
            //    {
            //        throw new Exception("Cannot decrease the number of rows or seats. Tickets have already been sold.");
            //    }
            //}
        }
    }
}
