using System;
using Akka.Actor;
using static System.Console;

namespace WinTail
{
    internal class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";

        public const string StartCommand = "start";

        private readonly ActorSelection _validationActor;

        public ConsoleReaderActor() => _validationActor = Context.ActorSelection(Names.TopLevelActorPath(Names.ValidatorActor));

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                ConsoleWriter.PrintInstructions();
            }
            GetAndValidateInput();
        }
        
        private void GetAndValidateInput()
        {
            var message = ReadLine();
            if (string.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                Context.System.Terminate();
            }
            else
            {
                _validationActor.Tell(message);
            }
        }
    }
}