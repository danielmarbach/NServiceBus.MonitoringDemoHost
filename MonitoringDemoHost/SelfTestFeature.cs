using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;

namespace Store.Shared.SelfTest
{
    class SelfTestFeature : Feature
    {
        public SelfTestFeature()
        {
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            context.RegisterStartupTask(new MyStartupTask());
        }
    }

    class MyStartupTask : FeatureStartupTask
    {
        Random random = new Random(Guid.NewGuid().GetHashCode());
        Task loop;
        bool stop;

        protected override Task OnStart(IMessageSession session)
        {
            loop = Task.Run(() => Loop(session));
            return Task.CompletedTask;
        }

        async Task Loop(IMessageSession session)
        {
            var min = random.Next(100);
            var max = random.Next(min, 1000);

            while (!stop)
            {
                await session.SendLocal(new Ping()).ConfigureAwait(false);
#pragma warning disable 4014
                Console.Out.WriteAsync("s");
#pragma warning restore 4014
                var delay = random.Next(min, max);
                await Task.Delay(TimeSpan.FromMilliseconds(delay));
            }
        }
        protected override Task OnStop(IMessageSession session)
        {
            stop = true;
            return loop;
        }

    }

    class Ping : IMessage
    {
    }

    class PingHandler : IHandleMessages<Ping>
    {
        static readonly ILog Log = LogManager.GetLogger(nameof(SelfTestFeature));

        public Task Handle(Ping message, IMessageHandlerContext context)
        {
            Console.Write(".");
            return Task.CompletedTask;
        }
    }
}
