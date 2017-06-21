using System;
using System.Windows.Forms;
using Akka.Actor;
using static ChartApp.Names;

namespace ChartApp
{
    static class Program
    {
        public static ActorSystem ChartActors;

        [STAThread]
        static void Main()
        {
            ChartActors = ActorSystem.Create(SystemName);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
