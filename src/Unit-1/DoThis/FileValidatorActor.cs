using System.IO;
using Akka.Actor;
using static WinTail.Messages;

namespace WinTail
{
    public class FileValidatorActor: UntypedActor
    {
        private readonly IActorRef _writerActor;
        private readonly ActorSelection _tailCoordinatorActor;

        public FileValidatorActor(IActorRef writerActor)
        {
            _writerActor = writerActor;
            _tailCoordinatorActor = Context.ActorSelection(Names.TopLevelActorPath(Names.TailCoordinatorActor));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case string msg when string.IsNullOrEmpty(msg):
                    _writerActor.Tell(NoInputMessage);
                    Sender.Tell(ContinueProcessingMessage);
                    break;
                case string msg when IsFileUri(msg):
                    _writerActor.Tell(new Success.InputSuccess("Starting processing for " + msg));
                    _tailCoordinatorActor.Tell(new TailCoordinatorActor.StartTail(msg,_writerActor));
                    break;
                default:
                    _writerActor.Tell(new Error.ValidationError(message + " is not an existing URI on disk."));
                    Sender.Tell(ContinueProcessingMessage);
                    break;
            }
        }
       
        private static bool IsFileUri(string path) =>  File.Exists(path);
    }
}