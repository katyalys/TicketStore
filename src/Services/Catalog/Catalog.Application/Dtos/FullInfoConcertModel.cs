﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos
{
    public class FullInfoConcertModel
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Perfomer { get; set; }

        public string GenreName { get; set; }

      //  public int? PlaceId { get; set; }
        public PlaceModel? Place { get; set; }
    }
}
