using Order.Domain.Entities;
using Order.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task Add(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        void Update(T entity);
        Task SaveAsync();
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    }
}
