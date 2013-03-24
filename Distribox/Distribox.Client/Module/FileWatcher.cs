using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Distribox.CommonLib;
using System.Diagnostics;

namespace Distribox.Client.Module
{
    public class FileChangedEventArgs
    {
        public WatcherChangeTypes ChangeType { get; set; }
        public String FullPath { get; set; }
        public String Name { get; set; }
        public String OldFullPath { get; set; }
        public String OldName { get; set; }
        public String SHA1 { get; set; }
        public String DataPath { get; set; }
        public DateTime When { get; set; }
        public Boolean IsDirectory { get; set; }
    }

    public class FileWatcher
    {
        public delegate void FileSystemChangedHandler(FileChangedEventArgs e);
        public event FileSystemChangedHandler Changed;
        public event FileSystemChangedHandler Created;
        public event FileSystemChangedHandler Renamed;
        public event FileSystemChangedHandler Deleted;

        public delegate void IdleHandler();
        public event IdleHandler Idle;

        private const int TIME_INTERVAL = 1000;

        private Queue<FileSystemEventArgs> _event_queue = new Queue<FileSystemEventArgs>();
        private System.Timers.Timer timer = new System.Timers.Timer(TIME_INTERVAL);
        private String _root;
        private String _hidden_path
        {
            get { return _root + ".Distribox"; }
        }

        private String _data_path
        {
            get { return _root + ".Distribox/data/"; }
        }

        public FileWatcher(String root)
        {
            this._root = root;

            FileSystemWatcher wather = new FileSystemWatcher();
            wather.Path = root;
            wather.Changed += OnWatcherEvent;
            wather.Created += OnWatcherEvent;
            wather.Renamed += OnWatcherEvent;
            wather.Deleted += OnWatcherEvent;
            wather.Filter = "*.*";
            wather.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            wather.EnableRaisingEvents = true;
            wather.IncludeSubdirectories = true;

            timer.Elapsed += OnTimerEvent;
            timer.AutoReset = true;
            timer.Start();
        }

        void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_event_queue)
            {
                if (_event_queue.Count() == 0) return;
            }
            timer.Stop();
            while (true)
            {
                FileSystemEventArgs args = null;
                lock (_event_queue)
                {
                    if (_event_queue.Count() == 0) break;
                    args = _event_queue.Dequeue();
                }
                switch (args.ChangeType)
                {
                    case WatcherChangeTypes.Changed:
                        OnChangedEvent(null, args);
                        break;
                    case WatcherChangeTypes.Created:
                        OnCreatedEvent(null, args);
                        break;
                    case WatcherChangeTypes.Deleted:
                        OnDeletedEvent(null, args);
                        break;
                    case WatcherChangeTypes.Renamed:
                        OnRenamedEvent(null, args as RenamedEventArgs);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            Idle();
            timer.Start();
        }

        void OnWatcherEvent(object sender, FileSystemEventArgs e)
        {
            if (!e.Name.StartsWith(".Distribox"))
            {
                lock (_event_queue)
                {
                    _event_queue.Enqueue(e);
                }
            }
        }

        public void OnTimerEvent()
        {
            Boolean changed = false;
                while (true)
                {
                    FileSystemEventArgs e = null;
                    lock (_event_queue)
                    {
                        if (_event_queue.Count() == 0) break;
                        e = _event_queue.Dequeue();
                    }
                    switch (e.ChangeType)
                    {
                        case WatcherChangeTypes.Changed:
                            OnChangedEvent(null, e);
                            break;
                        case WatcherChangeTypes.Created:
                            OnCreatedEvent(null, e);
                            break;
                        case WatcherChangeTypes.Deleted:
                            OnDeletedEvent(null, e);
                            break;
                        case WatcherChangeTypes.Renamed:
                            OnRenamedEvent(null, e as RenamedEventArgs);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    changed = true; 
                }

                if (changed && Idle != null)
                {
                    Idle();
                    changed = false;
                }
        }

        void OnDeletedEvent(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWith(_hidden_path))
            {
                Console.WriteLine("Deleted: {0}", e.Name);

                FileChangedEventArgs new_event = TranslateEvent(e);
                if (Deleted != null) Deleted(new_event);
            }
        }

        void OnRenamedEvent(object sender, RenamedEventArgs e)
        {
            if (!e.FullPath.StartsWith(_hidden_path))
            {
                Console.WriteLine("Renamed: {0} -> {1}", e.OldName, e.Name);

                FileChangedEventArgs new_event = TranslateEvent(e);
                
                if (!new_event.IsDirectory)
                {
                    new_event.SHA1 = CommonHelper.GetSHA1Hash(e.FullPath);
                    new_event.DataPath = _data_path + new_event.SHA1;
                    if (!File.Exists(new_event.DataPath))
                    {
                        File.Copy(new_event.FullPath, new_event.DataPath);
                    }
                }

                if (Renamed != null) Renamed(new_event);
            }
        }

        int count = 0;
        void OnCreatedEvent(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWith(_hidden_path))
            {
                Console.WriteLine("Create: {0}  {1}", e.Name, count++);

                FileChangedEventArgs new_event = TranslateEvent(e);

                if (Created != null) Created(new_event);
            }
        }

        void OnChangedEvent(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWith(_hidden_path))
            {
                Console.WriteLine("Changed: {0}", e.Name);
                FileChangedEventArgs new_event = TranslateEvent(e);

                if (!new_event.IsDirectory)
                {
                    new_event.SHA1 = CommonHelper.GetSHA1Hash(e.FullPath);
                    new_event.DataPath = _data_path + new_event.SHA1;
                    if (!File.Exists(new_event.DataPath))
                    {
                        File.Copy(new_event.FullPath, new_event.DataPath);
                    }
                }

                if (Changed != null) Changed(new_event);
            }
        }

        private FileChangedEventArgs TranslateEvent(FileSystemEventArgs e)
        {
            FileChangedEventArgs new_event = new FileChangedEventArgs();
            new_event.ChangeType = e.ChangeType;
            new_event.FullPath = e.FullPath;
            new_event.Name = e.Name;
            new_event.When = DateTime.Now;

            if (e.ChangeType != WatcherChangeTypes.Deleted)
            {
                FileInfo info = new FileInfo(new_event.FullPath);
                new_event.IsDirectory = (info.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
            }
            if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                new_event.OldName = ((RenamedEventArgs)e).OldName;
                new_event.FullPath = ((RenamedEventArgs)e).OldFullPath;
            }
            return new_event;
        }
    }
}
