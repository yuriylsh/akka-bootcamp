using System;
using Akka.Actor;
using static WinTail.ConsoleWriter;

namespace WinTail
{
    internal class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message as string)
            {
                    case string s when string.IsNullOrEmpty(s):
                        WriteLine(ConsoleColor.DarkYellow, "Please provide an input.\n");
                        break;
                    case string s when s.Length % 2 == 0:
                        WriteLine(ConsoleColor.Red, "Your string had an even # of characters.\n");
                        break;
                    default:
                        WriteLine(ConsoleColor.Green, "Your string had an odd # of characters.\n");
                        break;
            }
        }
    }
}
