using FluentValidation;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.FluentValidation
{
    public class TicketDetailedValidator : AbstractValidator<TicketsDetailedQuery>
    {
        public TicketDetailedValidator()
        {
            RuleFor(x => x.OrderId)
                .NotNull()
                .GreaterThan(0).WithMessage("Order id must be greater than 0.");
        }
    }
}
