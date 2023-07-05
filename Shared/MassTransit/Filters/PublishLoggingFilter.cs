using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MassTransit.Filters
{
    public class PublishLoggingFilter<T> : IFilter<PublishContext<T>> where T : class
    {
        private readonly ILogger<PublishLoggingFilter<T>> _logger;

        public PublishLoggingFilter(ILogger<PublishLoggingFilter<T>> logger)
        {
            _logger = logger;
        }

        public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            _logger.LogInformation("Publishing message: {@message}", context);
            await next.Send(context);
            _logger.LogInformation("Message published: {@message}", context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("logging");
        }
    }
}
