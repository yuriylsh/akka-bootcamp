using Akka.Actor;
using Akka.Configuration;

namespace WinTail
{
    internal class Program
    {
        private static void Main()
        {
            var actorSystem = ActorSystem.Create(Names.SystemName, ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true"));

            var writerActor = actorSystem.ActorOf(Props.Create<ConsoleWriterActor>(), Names.WriterActor);
            actorSystem.ActorOf(Props.Create<TailCoordinatorActor>(), Names.TailCoordinatorActor);
            actorSystem.ActorOf(Props.Create<FileValidatorActor>(writerActor), Names.ValidatorActor);
            var readerActor = actorSystem.ActorOf(Props.Create<ConsoleReaderActor>(), Names.ReaderActor);
            readerActor.Tell(ConsoleReaderActor.StartCommand);

            actorSystem.WhenTerminated.Wait();
        }
    }
}
