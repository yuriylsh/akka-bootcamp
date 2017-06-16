using System;
using Akka.Actor;

namespace WinTail
{
    internal class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
        private readonly IActorRef _writerActor;

        public ConsoleReaderActor(IActorRef writerActor) => _writerActor = writerActor;

        protected override void OnReceive(object message)
        {
            var read = Console.ReadLine();
            if (ExitCommand.Equals(read, StringComparison.OrdinalIgnoreCase))
            {
                Context.System.Terminate();
                return;
            }
            _writerActor.Tell(read);
            Self.Tell(
                "Send a message to itself after sending a message to ConsoleWriterActor. This is what keeps the read loop going.");
        }
    }
}