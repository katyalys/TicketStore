using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.Specification
{
    public interface ISpecification<T>
    {
        List<Expression<Func<T, object>>> Includes { get; }
    }
}
