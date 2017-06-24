using System;
using System.Windows.Forms;
using Akka.Actor;

namespace GithubActors
{
    static class Program
    {
        public static ActorSystem GithubActors;

        [STAThread]
        static void Main()
        {
            GithubActors = ActorSystem.Create("GithubActors");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new GithubAuth());
        }
    }
}
