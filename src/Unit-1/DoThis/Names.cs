namespace WinTail
{
    public class Names
    {
        public const string SystemName = "MyActorSystem";

        public const string TailCoordinatorActor = "tailCoordinatorActor";

        public const string ValidatorActor = "validatorActor";

        public const string WriterActor = "writerActor";

        public const string ReaderActor = "readerActor";

        public static string TopLevelActorPath(string actorName)
            => string.Concat("akka://", SystemName, "/user/", actorName);
    }
}