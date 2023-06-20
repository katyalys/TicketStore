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
        Task<Result<IReadOnlyList<Concert>>> GetCurrentConcerts(ConcertsSpecParam concertsSpec, bool isDescOrder);
        Task<Result<Concert>> GetConcert(int id);
        Task<Result<IReadOnlyList<Concert>>> GetSearchedConcerts(string searchTerm);
        Task<Result> AddConcertAsync(Concert concert);
        Task<Result> DeleteConcertAsync(int concertId);
        Task<Result> UpdateConcertAsync(Concert concert);
        Task<Result<IReadOnlyList<Concert>>> GetAllConcerts();
    }
}
