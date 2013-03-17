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
        public WatcherChangeTypes ChangeType;
        public String FullPath;
        public String Name;
        public String OldFullPath;
        public String OldName;
        public String SHA1;
        public String DataPath;
        public DateTime When;
        public Boolean IsDirectory;
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

        public Queue<FileSystemEventArgs> EventQueue = new Queue<FileSystemEventArgs>();

        private String root;

        private String hidden_path
        {
            get { return root + ".Distribox"; }
        }

        private String data_path
        {
            get { return root + ".Distribox\\data\\"; }
        }

        public FileWatcher(String root)
        {
            this.root = root;

            FileSystemWatcher wather = new FileSystemWatcher();
            wather.Path = root;
            wather.Changed += watcher_event;
            wather.Created += watcher_event;
            wather.Renamed += watcher_event;
            wather.Deleted += watcher_event;
            wather.Filter = "*.*";
            wather.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            wather.EnableRaisingEvents = true;
            wather.IncludeSubdirectories = true;
        }

        void watcher_event(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWith(hidden_path))
            {
                lock (EventQueue)
                {
                    EventQueue.Enqueue(e);
                }
            }
        }

        public void WaitForEvent()
        {
            Boolean changed = false;
            while (true)
            {
                while (true)
                {
                    FileSystemEventArgs e = null;
                    lock (EventQueue)
                    {
                        if (EventQueue.Count() == 0) break;
                        e = EventQueue.Dequeue();
                    }
                    switch (e.ChangeType)
                    {
                        case WatcherChangeTypes.Changed:
                            watcher_Changed(null, e);
                            break;
                        case WatcherChangeTypes.Created:
                            watcher_Created(null, e);
                            break;
                        case WatcherChangeTypes.Deleted:
                            watcher_Deleted(null, e);
                            break;
                        case WatcherChangeTypes.Renamed:
                            watcher_Renamed(null, e as RenamedEventArgs);
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

                Thread.Sleep(100);
            }
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWith(hidden_path))
            {
                Console.WriteLine("Deleted: {0}", e.Name);

                FileChangedEventArgs new_event = translate(e);
                if (Deleted != null) Deleted(new_event);
            }
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!e.FullPath.StartsWith(hidden_path))
            {
                Console.WriteLine("Renamed: {0} -> {1}", e.OldName, e.Name);

                FileChangedEventArgs new_event = translate(e);
                
                if (!new_event.IsDirectory)
                {
                    new_event.SHA1 = CommonHelper.GetSHA1Hash(e.FullPath);
                    new_event.DataPath = data_path + new_event.SHA1;
                    if (!File.Exists(new_event.DataPath))
                    {
                        File.Copy(new_event.FullPath, new_event.DataPath);
                    }
                }

                if (Renamed != null) Renamed(new_event);
            }
        }

        int count = 0;
        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWith(hidden_path))
            {
                Console.WriteLine("Create: {0}  {1}", e.Name, count++);

                FileChangedEventArgs new_event = translate(e);

                if (Created != null) Created(new_event);
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.StartsWith(hidden_path))
            {
                Console.WriteLine("Changed: {0}", e.Name);
                FileChangedEventArgs new_event = translate(e);

                if (!new_event.IsDirectory)
                {
                    new_event.SHA1 = CommonHelper.GetSHA1Hash(e.FullPath);
                    new_event.DataPath = data_path + new_event.SHA1;
                    if (!File.Exists(new_event.DataPath))
                    {
                        File.Copy(new_event.FullPath, new_event.DataPath);
                    }
                }

                if (Changed != null) Changed(new_event);
            }
        }

        private FileChangedEventArgs translate(FileSystemEventArgs e)
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
