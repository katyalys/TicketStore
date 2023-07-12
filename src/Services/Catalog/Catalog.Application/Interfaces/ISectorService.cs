using Catalog.Application.Dtos.SectorDtos;
using Catalog.Domain.ErrorModels;

namespace Catalog.Application.Interfaces
{
    public interface ISectorService
    {
        Task<Result<List<SectorInfoDto>>> ListAllPossibleSeatsAsync(int placeId);
        Task<Result> AddSectorAsync(SectorFullInffoDto sectorAddDto);
        Task<Result> DeleteSectorAsync(int sectorId);
        Task<Result> UpdateSectorAsync(SectorFullInffoDto sectorFullInffoDto);
    }
}
