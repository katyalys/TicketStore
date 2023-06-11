using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos
{
    public class ConcertsShortViewModel
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Perfomer { get; set; }

        public PlaceModel? Place { get; set; }
    }
}
