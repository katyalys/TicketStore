using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.SectorsSpecifications
{
    //public class TicketsSoldForSectorSpec : BaseSpecification<Sector>
    //{
    //    public TicketsSoldForSectorSpec(int sectorId) : base(t => t.Id == sectorId)
    //    {
    //        AddInclude(t => t.Tickets);
    //        AddCriteria(t => t.Tickets.Any(t => t.StatusId == 1));
    //    }
    //}

    public class TicketsSoldForSectorSpec : BaseSpecification<Ticket>
    {
        public TicketsSoldForSectorSpec(int sectorId)
        {
            AddCriteria(t => t.StatusId == 1 && t.SectorId == sectorId);
        }
    }
}
