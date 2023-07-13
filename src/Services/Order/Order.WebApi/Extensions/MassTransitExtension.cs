using MassTransit;
using Shared.EventBus.Messages.Constants;
using Shared.MassTransit.Filters;
using System.Reflection;

namespace Order.WebApi.Extensions
{
    public static class MassTransitExtension
    {
        public static void AddMassTransitConfig(this IServiceCollection services, IConfiguration config)
        {
            services.AddMassTransit(x =>
            {
                var host = config["RabbitMQ:Host"];
                var virtualHost = config["RabbitMQ:VirtualHost"];
                var username = config["RabbitMQ:Username"];
                var password = config["RabbitMQ:Password"];

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, virtualHost, h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.UsePublishFilter(typeof(PublishLoggingFilter<>), context);
                });
            });
        }
    }
}
