﻿using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Dtos.TicketDtos
{
    public class TicketAddDto
    {
        public int ConcertId { get; set; }
        public int SectorId { get; set; }
        public int StatusId { get; set; } 
        public int Row { get; set; }
        public int Seat { get; set; }
    }
}
