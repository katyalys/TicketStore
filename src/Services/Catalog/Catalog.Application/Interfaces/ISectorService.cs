using Catalog.Application.Dtos.SectorDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface ISectorService
    {
        Task<List<SectorInfoDto>> ListAllPossibleSeats(int placeId);
        Task AddSector(SectorFullInffoDto sectorAddDto);
        Task DeleteSector(int sectorId);
        Task UpdateSectorAsync(SectorFullInffoDto sectorFullInffoDto);
    }
}
