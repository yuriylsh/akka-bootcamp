﻿using System;
using System.IO;
using Akka.Actor;

namespace WinTail
{
    public class FileObserver: IDisposable
    {
        private readonly IActorRef _tailActor;
        private FileSystemWatcher _watcher;
        private readonly string _fileDir;
        private readonly string _fileNameOnly;

        public FileObserver(IActorRef tailActor, string absoluteFilePath)
        {
            _tailActor = tailActor;
            _fileDir = Path.GetDirectoryName(absoluteFilePath);
            _fileNameOnly = Path.GetFileName(absoluteFilePath);
        }

        public void Start()
        {
            _watcher =
                new FileSystemWatcher(_fileDir, _fileNameOnly)
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                };

            _watcher.Changed += OnFileChanged;
            _watcher.Error += OnFileError;
            _watcher.EnableRaisingEvents = true; // start watching

            void OnFileError(object sender, ErrorEventArgs e) 
                => _tailActor.Tell(
                    new TailActor.FileError(_fileNameOnly, e.GetException().Message),
                    ActorRefs.NoSender);
        
            void OnFileChanged(object sender, FileSystemEventArgs e)
            {
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    _tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
                }
            }
        }

        public void Dispose() => _watcher?.Dispose();
    }
}