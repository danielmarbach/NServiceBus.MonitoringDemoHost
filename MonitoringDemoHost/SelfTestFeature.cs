﻿using System;
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
        static Random random = new Random();
        Task loop;
        bool stop;

        protected override Task OnStart(IMessageSession session)
        {
            loop = Loop(session);
            return Task.CompletedTask;
        }

        async Task Loop(IMessageSession session)
        {
            int min, max;
            lock (random)
            {
                min = random.Next(100);
                max = random.Next(min, 1000);
            }

            while (!stop)
            {
                await session.SendLocal(new Ping()).ConfigureAwait(false);
                int delay;
                lock (random)
                {
                    delay = random.Next(min, max);
                }
                await Task.Delay(TimeSpan.FromMilliseconds(delay)).ConfigureAwait(false);
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
            return Task.CompletedTask;
        }
    }
}
