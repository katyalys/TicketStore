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
        public SectorsToDeleteSpec(int placeId, int sectorName)
        {
            AddCriteria(s => s.PlaceId == placeId);
            AddCriteria(s => s.Name == (SectorName)sectorName);
            AddCriteria(s => s.Tickets != null && !s.Tickets.Any(t => t.IsDeleted));
        }
    }
}
