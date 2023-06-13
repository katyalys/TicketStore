﻿using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Specification.SectorsSpecifications
{
    public class SectorsByPlaceSpec : BaseSpecification<Sector>
    {
        public SectorsByPlaceSpec(int placeId)
        {
            AddCriteria(s => s.PlaceId == placeId);
            AddInclude(s => s.Place);
        }

        public SectorsByPlaceSpec(int placeId, SectorName sectorName)
        {
            AddCriteria(s => s.PlaceId == placeId && s.Name == sectorName);
            AddInclude(s => s.Place);
           // AddCriteria(s => s.Name == sectorName);
        }

        //public SectorsByPlaceSpec(SectorName sectorName, int placeId)
        //{
        //    AddCriteria(s => s.Name == sectorName && s.PlaceId == placeId);
        //}
    }
}
