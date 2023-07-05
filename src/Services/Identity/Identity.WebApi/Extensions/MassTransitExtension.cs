using Identity.Application.MassTransit.Consumers;
using MassTransit;
using Shared.EventBus.Messages.Constants;
using Shared.MassTransit.Filters;
using System.Reflection;

namespace Identity.WebApi.Extensions
{
    public static class MassTransitExtension
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                var assembly = Assembly.GetAssembly(typeof(EmailUpdatedInfoConsumer));
                x.AddConsumers(assembly);

                string host = configuration["RabbitMQ:Host"];
                var virtualHost = configuration["RabbitMQ:VirtualHost"];
                var username = configuration["RabbitMQ:Username"];
                var password = configuration["RabbitMQ:Password"];

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, virtualHost, h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.UseConsumeFilter(typeof(ConsumeLoggingFilter<>), context);

                    cfg.ReceiveEndpoint(EventBusConstants.EmailWithChangesQueue, x =>
                    {
                        x.ConfigureConsumer<EmailUpdatedInfoConsumer>(context);
                    });
                });
            });
        }
    }
}
