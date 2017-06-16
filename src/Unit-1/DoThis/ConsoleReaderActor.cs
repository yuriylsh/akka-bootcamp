using System;
using Akka.Actor;
using static System.Console;
using static WinTail.Messages;

namespace WinTail
{
    internal class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";

        public const string StartCommand = "start";

        private readonly IActorRef _writerActor;

        public ConsoleReaderActor(IActorRef writerActor) => _writerActor = writerActor;

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartCommand:
                    ConsoleWriter.PrintInstructions();
                    break;
                case Error.InputError error:
                    SendMessageToWriter(error);
                    break;
            }
            GetAndValidateInput();
        }

        private void SendMessageToWriter(object message) => _writerActor.Tell(message);
        
        private void GetAndValidateInput()
        {
            switch (ReadLine())
            {
                    case string s when string.IsNullOrEmpty(s):
                        Self.Tell(NoInputMessage);
                        break;
                    case string s when ExitCommand.Equals(s, StringComparison.OrdinalIgnoreCase):
                        Context.System.Terminate();
                        break;
                    case string s when IsValid(s):
                        SendMessageToWriter(InputSuccessMessage);
                        Self.Tell(ContinueProcessingMessage);
                        break;
                    default:
                        Self.Tell(ValidationErrorMessage);
                        break;
            }
        }

        private static bool IsValid(string message) => message.Length % 2 == 0;
    }
}