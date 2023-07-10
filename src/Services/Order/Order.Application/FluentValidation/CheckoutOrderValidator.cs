using FluentValidation;
using Order.Application.Features.Orders.Commands.CheckoutOrder;

namespace Order.Application.FluentValidation
{
    public class CheckoutOrderValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderValidator()
        {
            RuleFor(x => x.Status)
              .NotEmpty();

            RuleFor(x => x.OrderDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(x => x.TicketIds)
                .NotEmpty()
                .ForEach(ticketIdRule => ticketIdRule.GreaterThan(0));

            RuleFor(x => x.TotalPrice)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
