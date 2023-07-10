using FluentValidation;
using Order.Application.Features.Orders.Commands.CancelOrder;

namespace Order.Application.FluentValidation
{
    public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderValidator()
        {
            RuleFor(x => x.OrderId)
                .NotNull()
                .GreaterThan(0).WithMessage("Order id must be greater than 0.");
        }
    }
}
