using Catalog.Domain.Entities;
using Catalog.Domain.Specification;

namespace Catalog.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        void Delete(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpecAsync(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        void Update(T entity);
        void DeleteRange(IReadOnlyList<T> entities);
    }
}
