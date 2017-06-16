using System;
using Akka.Actor;
using static System.Console;

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
                case Messages.Error.InputError error:
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
                        Self.Tell(new Messages.Error.NullInputError("No input received."));
                        break;
                    case string s when ExitCommand.Equals(s, StringComparison.OrdinalIgnoreCase):
                        Context.System.Terminate();
                        break;
                    case string s when IsValid(s):
                        SendMessageToWriter(new Messages.Success.InputSuccess("Thank you! Message was valid."));
                        Self.Tell(new Messages.Neutral.ContinueProcessing());
                        break;
                    default:
                        Self.Tell(new Messages.Error.ValidationError("Invalid: input had odd number of characters."));
                        break;
            }
        }

        private static bool IsValid(string message) => message.Length % 2 == 0;
    }
}