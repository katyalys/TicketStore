﻿using Catalog.Domain.ErrorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Entities
{
    public class Ticket: BaseEntity
    {
        public int? ConcertId { get; set; }
        public Concert? Concert { get; set; }

        public int? SectorId { get; set; }
        public Sector? Sector { get; set; }

        public int? StatusId { get; set; }
        public Status? Status { get; set; }

        public int Row { get; set; }
        public int Seat { get; set; }

        public string? CustomerId { get; set; }
    }
}
