using Akka.Actor;
using Akka.Configuration;

namespace WinTail
{
    internal class Program
    {
        private static void Main()
        {
            var actorSystem = ActorSystem.Create("MyActorSystem", ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true"));

            var writerActor = actorSystem.ActorOf(Props.Create<ConsoleWriterActor>(), "writerActor");
            var tailCoordinatorActor = actorSystem.ActorOf(Props.Create<TailCoordinatorActor>(), "tailCoordinatorActor");
            var validationActor = actorSystem.ActorOf(
                Props.Create<FileValidatorActor>(writerActor, tailCoordinatorActor), 
                "validationActor");
            var readerActor = actorSystem.ActorOf(Props.Create<ConsoleReaderActor>(validationActor), "readerActor");
            readerActor.Tell(ConsoleReaderActor.StartCommand);

            actorSystem.WhenTerminated.Wait();
        }
    }
}
