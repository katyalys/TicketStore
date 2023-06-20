using Catalog.Application.Dtos.SectorDtos;
using Catalog.Domain.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface ISectorService
    {
        Task<Result<List<SectorInfoDto>>> ListAllPossibleSeats(int placeId);
        Task<Result> AddSector(SectorFullInffoDto sectorAddDto);
        Task<Result> DeleteSector(int sectorId);
        Task<Result> UpdateSectorAsync(SectorFullInffoDto sectorFullInffoDto);
    }
}
