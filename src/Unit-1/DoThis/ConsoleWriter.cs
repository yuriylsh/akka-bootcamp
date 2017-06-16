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

        public static void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.WriteLine("Some entries will pass validation, and some won't...\n\n");
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }
    }
}