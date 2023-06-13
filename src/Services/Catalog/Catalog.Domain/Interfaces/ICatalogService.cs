//using Catalog.Domain.Entities;
//using Catalog.Domain.Specification;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Catalog.Domain.Interfaces
//{
//    public interface ICatalogService
//    {
//        Task<IReadOnlyList<Concert>> GetCurrentConcerts(ConcertsSpecParam concertsSpec, bool isDescOrder);
//        Task<Concert> GetConcert(int id);
//        Task<IReadOnlyList<Concert>> GetSearchedConcerts(string searchTerm);
//        Task AddConcertAsync(Concert concert);
//        Task DeleteConcertAsync(int concertId);
//        Task UpdateConcertAsync(Concert concert);
//        Task<IReadOnlyList<Concert>> GetAllConcerts();
//    }
//}
