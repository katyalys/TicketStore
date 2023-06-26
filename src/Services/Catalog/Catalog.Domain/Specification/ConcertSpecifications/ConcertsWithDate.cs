using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.ConcertSpecifications
{
    public class ConcertsWithDate : BaseSpecification<Concert>
    {
        public ConcertsWithDate(DateTime currentDate)
        {
            AddCriteria(c => c.Date < currentDate && c.IsDeleted == false);
            AddInclude(c => c.Tickets);
        }
    }
}
