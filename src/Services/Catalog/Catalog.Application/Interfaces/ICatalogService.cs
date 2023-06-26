using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Specification;
using Catalog.Domain.Specification.ConcertSpecifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Interfaces
{
    public interface ICatalogService
    {
        Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetCurrentConcerts(ConcertsSpecParam concertsSpec, bool isDescOrder);
        Task<Result<FullInfoConcertDto>> GetConcert(int id);
        Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetSearchedConcerts(string searchTerm);
        Task<Result> AddConcertAsync(FullInfoConcertDto fullInfoConcertModel);
        Task<Result> DeleteConcertAsync(int concertId);
        Task<Result> UpdateConcertAsync(FullInfoConcertDto concertFullInfo, int idConcert);
        Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetAllConcerts();
    }
}
