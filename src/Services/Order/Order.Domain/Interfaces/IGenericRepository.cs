using Order.Domain.Entities;
using Order.Domain.Specification;

namespace Order.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        void Update(T entity);
        Task SaveAsync();
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<T> GetEntityWithSpec(ISpecification<T> spec);
    }
}
