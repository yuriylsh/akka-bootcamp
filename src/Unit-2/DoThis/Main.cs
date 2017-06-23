using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Akka.Actor;
using ChartApp.Actors;

namespace ChartApp
{
    public partial class Main : Form
    {
        private IActorRef _chartActor;

        private IActorRef _coordinatorActor;

        private Dictionary<CounterType, IActorRef> _toggleActors = new Dictionary<CounterType, IActorRef>();

        private static readonly ButtonToggleActor.Toggle ToggleButtonMessage = new ButtonToggleActor.Toggle();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), Names.ChartinActorName);
            _chartActor.Tell(new ChartingActor.InitializeChart(null)); 

            _coordinatorActor = Program.ChartActors.ActorOf(Props.Create(() =>
                new PerformanceCounterCoordinatorActor(_chartActor)), Names.CountersActorName);

            // CPU button toggle actor
            _toggleActors[CounterType.Cpu] = Program.ChartActors.ActorOf(CreateButtonToggleProps(btnCpu, CounterType.Cpu));

            // MEMORY button toggle actor
            _toggleActors[CounterType.Memory] = Program.ChartActors.ActorOf(CreateButtonToggleProps(btnMemory, CounterType.Memory));

            // DISK button toggle actor
            _toggleActors[CounterType.Disk] = Program.ChartActors.ActorOf(CreateButtonToggleProps(btnDisk, CounterType.Disk));

            // Set the CPU toggle to ON so we start getting some data
            _toggleActors[CounterType.Cpu].Tell(new ButtonToggleActor.Toggle());
        }

        private Props CreateButtonToggleProps(Button button, CounterType counterType) 
            => Props.Create(() => new ButtonToggleActor(_coordinatorActor, button, counterType, false))
                .WithDispatcher("akka.actor.synchronized-dispatcher");

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //shut down the charting actor
            _chartActor.Tell(PoisonPill.Instance);

            //shut down the ActorSystem
            Program.ChartActors.Terminate();
        }

        private void btnCpu_Click(object sender, EventArgs e)
        {
            _toggleActors[CounterType.Cpu].Tell(ToggleButtonMessage);
        }

        private void btnMemory_Click(object sender, EventArgs e)
        {
            _toggleActors[CounterType.Memory].Tell(ToggleButtonMessage);
        }

        private void btnDisk_Click(object sender, EventArgs e)
        {
            _toggleActors[CounterType.Disk].Tell(ToggleButtonMessage);
        }

        private void btnPauseResume_Click(object sender, EventArgs e)
        {

        }
    }
}
