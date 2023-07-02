using FluentValidation;
using Order.Application.Features.Orders.Commands.CancelTicket;

namespace Order.Application.FluentValidation
{
    public class CancelTicketValidator : AbstractValidator<CancelTicketCommand>
    {
        public CancelTicketValidator()
        {
            RuleFor(x => x.OrderId)
                .NotNull()
                .GreaterThan(0).WithMessage("Order id must be greater than 0.");

            RuleFor(x => x.TicketId)
               .NotNull()
               .GreaterThan(0).WithMessage("Order id must be greater than 0.");
        }
    }
}
