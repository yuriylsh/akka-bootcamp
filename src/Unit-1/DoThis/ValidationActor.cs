using Akka.Actor;
using static WinTail.Messages;

namespace WinTail
{
    public class ValidationActor: UntypedActor
    {
        private readonly IActorRef _writerActor;

        public ValidationActor(IActorRef writerActor)
        {
            _writerActor = writerActor;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case string s when string.IsNullOrEmpty(s):
                    _writerActor.Tell(NoInputMessage);
                    break;
                case string s when IsValid(s):
                    _writerActor.Tell(InputSuccessMessage);
                    break;
                default:
                    _writerActor.Tell(ValidationErrorMessage);
                    break;
            }

            // tell sender to continue doing its thing
            // (whatever that may be, this actor doesn't care)
            Sender.Tell(ContinueProcessingMessage);
        }

        private static bool IsValid(string message) => message.Length % 2 == 0;
    }
}