using FluentValidation;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;

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
