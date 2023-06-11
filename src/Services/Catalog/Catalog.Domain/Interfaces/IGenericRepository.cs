using Catalog.Domain.Entities;
using Catalog.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task Add(T entity);
        void Delete(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        void Update(T entity);

    }
}
