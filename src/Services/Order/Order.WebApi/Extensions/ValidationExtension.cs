using FluentValidation.AspNetCore;
using FluentValidation;
using Order.Application.Features.Orders.Commands.CancelOrder;
using Order.Application.FluentValidation;
using Order.Application.Features.Orders.Commands.CancelTicket;
using Order.Application.Features.Orders.Commands.CheckoutOrder;
using Order.Application.Features.Orders.Queries.TicketDetailedInfo;

namespace Order.WebApi.Extensions
{
    public static class ValidationExtension
    {
        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<CancelOrderCommand>, CancelOrderValidator>();
            services.AddScoped<IValidator<CancelTicketCommand>, CancelTicketValidator>();
            services.AddScoped<IValidator<CheckoutOrderCommand>, CheckoutOrderValidator>();
            services.AddScoped<IValidator<TicketsDetailedQuery>, TicketDetailedValidator>();

            return services;
        }
    }
}
