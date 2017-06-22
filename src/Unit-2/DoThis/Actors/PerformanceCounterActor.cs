using System;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class PerformanceCounterActor: UntypedActor
    {
        private readonly string _seriesName;

        private readonly Func<PerformanceCounter> _performanceCounterGenerator;

        private PerformanceCounter _counter;

        private readonly HashSet<IActorRef> _subscriptions;

        private readonly ICancelable _cancelPublishing;

        private static readonly TimeSpan InitialDelay = TimeSpan.FromMilliseconds(250);

        private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(250);

        private static readonly GatherMetrics GatherMetricsMessage = new GatherMetrics();

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
        {
            _seriesName = seriesName;
            _performanceCounterGenerator = performanceCounterGenerator;
            _subscriptions = new HashSet<IActorRef>();
            _cancelPublishing = new Cancelable(Context.System.Scheduler);
        }

        protected override void PreStart()
        {
            _counter = _performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(
                InitialDelay,
                Interval, 
                Self,
                GatherMetricsMessage, 
                Self, 
                _cancelPublishing);
        }

        protected override void PostStop()
        {
            try
            {
                //terminate the scheduled task
                _cancelPublishing.Cancel(false);
                _counter.Dispose();
            }
            finally
            {
                base.PostStop();
            }
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                    case GatherMetrics _:
                        PerformMetricsGathering();
                        break;
                    case SubscribeCounter sc:
                        _subscriptions.Add(sc.Subscriber);
                        break;
                    case UnsubscribeCounter uc:
                        _subscriptions.Remove(uc.Subscriber);
                        break;
            }
        }

        private void PerformMetricsGathering()
        {
            var metric = new Metric(_seriesName, _counter.NextValue());
            foreach (var sub in _subscriptions)
            {
                sub.Tell(metric);
            }
        }
    }


}