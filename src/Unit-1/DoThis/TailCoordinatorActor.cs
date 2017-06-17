using System;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor: UntypedActor
    {
        private const int MaxNumberOfRetries = 10;

        private static readonly TimeSpan WithinTimeRange = TimeSpan.FromSeconds(30);

        protected override void OnReceive(object message)
        {
            if (message is StartTail msg)
            {
                Context.ActorOf(Props.Create<TailActor>(msg.ReporterActor, msg.FilePath));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(MaxNumberOfRetries, WithinTimeRange, DirectiveDecider);

            Directive DirectiveDecider(Exception ex)
            {
                switch (ex)
                {
                        case ArithmeticException _:
                            //Maybe we consider ArithmeticException to not be application critical
                            //so we just ignore the error and keep going.
                            return Directive.Resume;
                        case NotSupportedException _:
                            //Error that we cannot recover from, stop the failing actor
                            return Directive.Stop;
                        default:
                            //In all other cases, just restart the failing actor
                            return Directive.Restart;
                }
            }
        }

        public class StartTail
        {
            public string FilePath { get; }

            public IActorRef ReporterActor { get; }

            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }
        }

        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; }
        }
    }
}