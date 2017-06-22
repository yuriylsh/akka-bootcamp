using System.Collections.Generic;
using Akka.Actor;
using static ChartApp.Generators;

namespace ChartApp.Actors
{
    public class PerformanceCounterCoordinatorActor: ReceiveActor
    {
        private readonly Dictionary<CounterType, IActorRef> _counterActors;

        private readonly IActorRef _chartingActor;

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor) : 
            this(chartingActor, new Dictionary<CounterType, IActorRef>())
        {
        }

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor, Dictionary<CounterType, IActorRef> counterActors)
        {
            _chartingActor = chartingActor;
            _counterActors = counterActors;

            Receive<Watch>(HandleWatch, null);

            Receive<Unwatch>(HandleUnwatch, null);
        }

        private void HandleWatch(Watch watch)
        {
            AddCounterIfDoesNotExists(watch.Counter);
            _chartingActor.Tell(new ChartingActor.AddSeries(SeriesGenerators[watch.Counter]()));
            _counterActors[watch.Counter].Tell(new SubscribeCounter(watch.Counter, _chartingActor));
        }

        private void AddCounterIfDoesNotExists(CounterType counterType)
        {
            if (!_counterActors.ContainsKey(counterType))
            {
                _counterActors[counterType] = Context.ActorOf(
                    Props.Create<PerformanceCounterActor>(counterType.ToString(), CounterGenerators[counterType]));
            }
        }

        private void HandleUnwatch(Unwatch unwatch) => RemoveCounterIfExists(unwatch.Counter);

        private void RemoveCounterIfExists(CounterType counterType)
        {
            if (_counterActors.ContainsKey(counterType))
            {
                _counterActors[counterType].Tell(new UnsubscribeCounter(counterType, _chartingActor));

                // remove this series from the ChartingActor
                _chartingActor.Tell(new ChartingActor.RemoveSeries(counterType.ToString()));
            }
        }

        public class Watch
        {
            public CounterType Counter { get; }

            public Watch(CounterType counter) => Counter = counter;
        }

        public class Unwatch
        {
            public CounterType Counter { get; }

            public Unwatch(CounterType counter) => Counter = counter;
        }
    }
}