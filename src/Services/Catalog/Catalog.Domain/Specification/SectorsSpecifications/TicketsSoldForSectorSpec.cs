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
        public TicketsSoldForSectorSpec(int sectorId)
        {
            AddCriteria(t => (t.StatusId == ((int)StatusTypes.Bought + 1) || t.StatusId == ((int)StatusTypes.Book + 1)) 
                                && t.SectorId == sectorId);
        }
    }
}
