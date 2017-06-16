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
            switch (Console.ReadLine())
            {
                case string s when ExitCommand.Equals(s, StringComparison.OrdinalIgnoreCase):
                    Exit();
                    break;
                case string read:
                    SendMessageToWriter(read);
                    ContinueLoop();
                    break;
            }
        }

        private static void Exit() => Context.System.Terminate();

        private void SendMessageToWriter(string message) => _writerActor.Tell(message);

        private void ContinueLoop() => Self.Tell("Send a message to itself. This is what keeps the read loop going.");
    }
}