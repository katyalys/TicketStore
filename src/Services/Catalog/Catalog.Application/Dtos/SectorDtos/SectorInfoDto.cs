using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos.SectorDtos
{
    public class SectorInfoDto
    {
        public string SectorName { get; set; }
        public int RowNumber { get; set; }
        public int RowSeatNumber { get; set; }
        public List<SeatRangeDto> SeatRanges { get; set; }
    }
}
