using System;

namespace WinTail
{
    internal static class ConsoleWriter
    {
        public static void WriteLine(ConsoleColor color, string message)
            => Write(color, message + Environment.NewLine);

        public static void Write(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void PrintInstructions() => Console.WriteLine("Please provide the URI of a log file on disk.\n");
    }
}