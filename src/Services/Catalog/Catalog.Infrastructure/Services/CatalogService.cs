using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class CatalogService: ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CatalogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IReadOnlyList<Concert>> GetCurrentConcerts(ConcertsSpecParam concertsSpec, bool isDescOrder)
        {
            var spec = new ConcertsFilterAndSort(concertsSpec, isDescOrder);
            var concerts = _unitOfWork.Repository<Concert>().ListAsync(spec);
            return concerts;
        }

        public Task<Concert> GetConcert(int id)
        {
            var spec = new ConcertFullInfo(id);
            var concert = _unitOfWork.Repository<Concert>().GetEntityWithSpec(spec);
            return concert;
        }

        public Task<IReadOnlyList<Concert>> GetSearchedConcerts(string searchTerm)
        {
            var spec = new ConcertsBySearchSpec(searchTerm);
            var concerts = _unitOfWork.Repository<Concert>().ListAsync(spec);
            return concerts;
        }

        public async Task AddConcertAsync(Concert concert)
        {
            await _unitOfWork.Repository<Concert>().Add(concert);
            await _unitOfWork.Complete();
        }

        public async Task DeleteConcertAsync(int concertId)
        {
            var concert = await _unitOfWork.Repository<Concert>().GetByIdAsync(concertId);

            if (concert.Tickets != null && concert.Tickets.Any(t => !t.IsDeleted))
            {
                throw new Exception("Cant delete because of exsisting tickets");
            }
            _unitOfWork.Repository<Concert>().Delete(concert);
            await _unitOfWork.Complete();
        }

        public async Task UpdateConcertAsync(Concert concert)
        {
            _unitOfWork.Repository<Concert>().Update(concert);
            await _unitOfWork.Complete();
        }

        public async Task<IReadOnlyList<Concert>> GetAllConcerts()
        {
            var spec = new ConcertFullInfo();
            var allConcerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);
            return allConcerts;
        }

    }
}
