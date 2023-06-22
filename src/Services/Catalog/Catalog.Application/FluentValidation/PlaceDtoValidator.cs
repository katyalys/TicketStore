using Catalog.Application.Dtos;
using Catalog.Application.Dtos.PlaceDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.FluentValidation
{
    public class PlaceDtoValidator : AbstractValidator<PlaceDto>
    {
        public PlaceDtoValidator()
        {
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required.");

            RuleFor(x => x.PlaceNumber)
                .GreaterThan(0).WithMessage("Place number must be greater than 0.");
        }
    }
}
