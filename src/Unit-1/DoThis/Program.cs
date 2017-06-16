using Akka.Actor;
using Akka.Configuration;

namespace WinTail
{
    internal class Program
    {
        private static void Main()
        {
            var actorSystem = ActorSystem.Create("MyActorSystem", ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true"));

            var writerActor = actorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var readerActor = actorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(writerActor)));
            readerActor.Tell(ConsoleReaderActor.StartCommand);

            actorSystem.WhenTerminated.Wait();
        }
    }
}
