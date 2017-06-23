using System;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ButtonToggleActor: UntypedActor
    {
        private readonly CounterType _myCounterType;
        private bool _isToggledOn;
        private readonly IActorRef _coordinatorActor;
        private readonly Action<string> _buttonTextSetter;
        private readonly string _myCounterTypeName;

        public ButtonToggleActor(IActorRef coordinatorActor, Action<string> buttonTextSetter, CounterType myCounterType, bool isToggledOn = false)
        {
            _coordinatorActor = coordinatorActor;
            _buttonTextSetter = buttonTextSetter;
            _isToggledOn = isToggledOn;
            _myCounterType = myCounterType;
            _myCounterTypeName = _myCounterType.ToString().ToUpperInvariant();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Toggle _ when _isToggledOn:
                    _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Unwatch(_myCounterType));
                    FlipToggle();
                    break;
                case Toggle _ when !_isToggledOn:
                    _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(_myCounterType));
                    FlipToggle();
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }

        private void FlipToggle()
        {
            _isToggledOn = !_isToggledOn;
            _buttonTextSetter(string.Concat(_myCounterTypeName, " (", _isToggledOn ? "ON" : "OFF", ")"));
        }

        public class Toggle { }
    }
}