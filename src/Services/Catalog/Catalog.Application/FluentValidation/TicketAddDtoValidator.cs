using Catalog.Application.Dtos.TicketDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.FluentValidation
{
    public class TicketAddDtoValidator : AbstractValidator<TicketAddDto>
    {
        public TicketAddDtoValidator()
        {
            RuleFor(x => x.ConcertId)
                .GreaterThan(0).WithMessage("Concert ID must be greater than 0.");

            RuleFor(x => x.SectorId)
                .GreaterThan(0).WithMessage("Sector ID must be greater than 0.");

            RuleFor(x => x.StatusId)
                .GreaterThan(0).WithMessage("Status ID must be greater than 0.");

            RuleFor(x => x.Row)
                .GreaterThan(0).WithMessage("Row must be greater than 0.");

            RuleFor(x => x.Seat)
                .GreaterThan(0).WithMessage("Seat must be greater than 0.");
        }
    }
}
