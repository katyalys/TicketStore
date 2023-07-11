using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Specification.ConcertSpecifications;

namespace Catalog.Application.Interfaces
{
    public interface ICatalogService
    {
        Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetCurrentConcertsAsync(ConcertsSpecParam concertsSpec, bool isDescOrder);
        Task<Result<FullInfoConcertDto>> GetConcertAsync(int id);
        Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetSearchedConcertsAsync(string searchTerm);
        Task<Result> AddConcertAsync(FullInfoConcertDto fullInfoConcertModel);
        Task<Result> DeleteConcertAsync(int concertId);
        Task<Result> UpdateConcertAsync(FullInfoConcertDto concertFullInfo, int idConcert);
        Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetAllConcertsAsync();
    }
}
