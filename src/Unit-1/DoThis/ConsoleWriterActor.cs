using System;
using Akka.Actor;
using static WinTail.ConsoleWriter;

namespace WinTail
{
    internal class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                    case Messages.Error.InputError error:
                        WriteLine(ConsoleColor.Red, error.Reason);
                        break;
                    case Messages.Success.InputSuccess success:
                        WriteLine(ConsoleColor.Green, success.Reason);
                        break;
                    default:
                        Console.WriteLine(message);
                        break;
            }
        }
    }
}
