using Catalog.Application.Dtos.PlaceDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos.ConcertDtos
{
    public class ConcertsShortViewDto
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Perfomer { get; set; }
        public string GenreName { get; set; }
        public PlaceDto? Place { get; set; }
    }
}
