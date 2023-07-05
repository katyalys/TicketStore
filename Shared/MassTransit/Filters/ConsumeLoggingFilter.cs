using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MassTransit.Filters
{
    public class ConsumeLoggingFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly ILogger<ConsumeLoggingFilter<T>> _logger;

        public ConsumeLoggingFilter(ILogger<ConsumeLoggingFilter<T>> logger)
        {
            _logger = logger;
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            _logger.LogInformation("Received message: {@message}", context);
            await next.Send(context);
            _logger.LogInformation("Processed message: {@message}", context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("logging");
        }
    }
}
