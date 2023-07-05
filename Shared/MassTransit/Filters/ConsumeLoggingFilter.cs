using MassTransit;
using Microsoft.Extensions.Logging;

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
