using System.Linq.Expressions;

namespace Order.Domain.Specification
{
    public interface ISpecification<T>
    {
        List<Expression<Func<T, object>>> Includes { get; }
    }
}
