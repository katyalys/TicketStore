using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.SectorsSpecifications
{
    public class SectorsToDeleteSpec : BaseSpecification<Sector>
    {
        public SectorsToDeleteSpec(int sectorId, int? placeId) : base(s => s.Id == sectorId)
        {
            AddInclude(s => s.Tickets);
            AddCriteria(s => s.PlaceId == placeId && s.Tickets.Any(t => t.IsDeleted == false));
        }
    }
}
