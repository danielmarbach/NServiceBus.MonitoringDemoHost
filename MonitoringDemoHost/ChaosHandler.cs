using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class ChaosHandler : IHandleMessages<object>
{
    readonly ILog Log = LogManager.GetLogger<ChaosHandler>();
    Random random = new Random(Guid.NewGuid().GetHashCode());
    readonly double threshold;

    public ChaosHandler()
    {
        threshold = random.NextDouble() * 0.50;
    }

    public Task Handle(object message, IMessageHandlerContext context)
    {
        var result = random.NextDouble();

        if (result < threshold) throw new Exception($"Random chaos ({threshold * 100:N}% failure)");
        return Task.FromResult(0);
    }
}
