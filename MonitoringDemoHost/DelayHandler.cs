using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class DelayHandler : IHandleMessages<object>
{
    static readonly ILog Log = LogManager.GetLogger<DelayHandler>();
    Random random = new Random(Guid.NewGuid().GetHashCode());
    readonly int max;

    public DelayHandler()
    {
        max = random.Next(100);
    }

    public Task Handle(object message, IMessageHandlerContext context)
    {
        var duration = random.Next(0, max);

        Log.InfoFormat("Delaying for {0}ms", duration);

        return Task.Delay(duration);
    }
}
