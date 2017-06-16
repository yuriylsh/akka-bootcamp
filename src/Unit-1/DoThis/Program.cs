using System;
﻿using Akka.Actor;
using Akka.Configuration;
using static WinTail.ConsoleWriter;

namespace WinTail
{
    internal class Program
    {
        public static ActorSystem MyActorSystem = ActorSystem.Create(
            "MyActorSystem",
            ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true"));

        private static void Main()
        {
            PrintInstructions();
            var writerActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var readerActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(writerActor)));
            readerActor.Tell(
                "Send an initial message to ConsoleReaderActor in order to get it to start reading from the console.");
            MyActorSystem.WhenTerminated.Wait();
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Write(ConsoleColor.DarkRed, " red ");
            Console.Write(" and others will appear as");
            Write(ConsoleColor.Green, " green! ");
            Console.WriteLine("\n\nType 'exit' to quit this application at any time.\n");
        }
    }
}
