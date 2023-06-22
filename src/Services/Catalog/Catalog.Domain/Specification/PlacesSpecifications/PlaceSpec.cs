using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.PlacesSpecifications
{
    public class PlaceSpec : BaseSpecification<Place>
    {
        public PlaceSpec(int id)
        : base(x => x.Id == id)
        {
            AddInclude(x => x.Concerts);
            AddCriteria(x => x.Id == id && x.Concerts.Any());
        }

        public PlaceSpec(Place place)
        {
            AddCriteria(x => x.City == place.City && x.Street == place.Street && x.PlaceNumber == place.PlaceNumber);
        }
    }
}
