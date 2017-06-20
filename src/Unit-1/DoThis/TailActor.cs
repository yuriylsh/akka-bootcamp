using System.IO;
using System.Text;
using Akka.Actor;

namespace WinTail
{
    public class TailActor: UntypedActor
    {
        private readonly IActorRef _reporterActor;

        private readonly string _filePath;

        private FileObserver _observer;

        private StreamReader _fileStreamReader;

        public TailActor(IActorRef reporterActor, string filePath)
        {
            _reporterActor = reporterActor;
            _filePath = filePath;
        }

        protected override void PreStart()
        {
            _observer = new FileObserver(Self, Path.GetFullPath(_filePath));
            _observer.Start();

            var fileStream = new FileStream(
                Path.GetFullPath(_filePath),
                FileMode.Open, 
                FileAccess.Read, 
                FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(fileStream, Encoding.UTF8);

            Self.Tell(new InitialRead(_filePath, _fileStreamReader.ReadToEnd()));
        }

        protected override void PostStop()
        {
            _observer.Dispose();
            _observer = null;
            _fileStreamReader.Close();
            _fileStreamReader.Dispose();
            base.PostStop();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                    case FileWrite _:
                        ReportFileWrite();
                        break;
                    case FileError fileError:
                        _reporterActor.Tell("Tail error: " + fileError.Reason);
                        break;
                    case InitialRead initialRead:
                        _reporterActor.Tell(initialRead.Text);
                        break;
            }

            void ReportFileWrite()
            {
                // this is assuming a log file type format that is append-only
                var text = _fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    _reporterActor.Tell(text);
                }
            }
        }

        public class FileWrite
        {
            public string FileName { get; }

            public FileWrite(string fileName) => FileName = fileName;
        }

        public class FileError
        {
            public string FileName { get; }

            public string Reason { get; }

            public FileError(string fileName, string reason)
            {
                FileName = fileName;
                Reason = reason;
            }
        }

        public class InitialRead
        {
            public string FileName { get; }

            public string Text { get; }

            public InitialRead(string fileName, string text)
            {
                FileName = fileName;
                Text = text;
            }
        }
    }
}
