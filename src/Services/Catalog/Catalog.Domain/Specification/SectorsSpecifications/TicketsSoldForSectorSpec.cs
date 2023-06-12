using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.SectorsSpecifications
{
    public class TicketsSoldForSectorSpec : BaseSpecification<Ticket>
    {
        public TicketsSoldForSectorSpec(int sectorId, int statusId)
        {
            AddCriteria(t => t.SectorId == sectorId && t.StatusId == statusId);
           // AddInclude(t => t.Sector);
        }
    }
}
