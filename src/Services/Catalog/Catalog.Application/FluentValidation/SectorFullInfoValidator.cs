using Catalog.Application.Dtos.SectorDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.FluentValidation
{
    public class SectorFullInfoValidator : AbstractValidator<SectorFullInffoDto>
    {
        public SectorFullInfoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Sector name is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Sector name is required.");

            RuleFor(x => x.RowNumber)
                .GreaterThan(0).WithMessage("Row number must be greater than 0.");

            RuleFor(x => x.RowSeatNumber)
                .GreaterThan(0).WithMessage("Row seat number must be greater than 0.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.PictureLink)
                .Must(BeAValidUri).When(x => !string.IsNullOrEmpty(x.PictureLink))
                .WithMessage("Invalid picture link.");

            RuleFor(x => x.PlaceId)
                .GreaterThan(0).WithMessage("Place ID must be greater than 0.");
        }

        private bool BeAValidUri(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out Uri result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
