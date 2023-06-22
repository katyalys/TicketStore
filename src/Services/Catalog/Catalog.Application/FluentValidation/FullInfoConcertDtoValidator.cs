using Catalog.Application.Dtos;
using Catalog.Application.Dtos.ConcertDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.FluentValidation
{
    public class FullInfoConcertDtoValidator : AbstractValidator<FullInfoConcertDto>
    {
        public FullInfoConcertDtoValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .Must(BeAValidDateTime).WithMessage("Invalid date.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Perfomer)
                .NotEmpty().WithMessage("Performer is required.");

            RuleFor(x => x.GenreName)
                .NotEmpty().WithMessage("Genre name is required.");

            RuleFor(x => x.Place)
                .NotNull().WithMessage("Place is required.")
                .SetValidator(new PlaceDtoValidator()); 
        }

        private bool BeAValidDateTime(DateTime date)
        {
            return date != default(DateTime);
        }
    }
}
