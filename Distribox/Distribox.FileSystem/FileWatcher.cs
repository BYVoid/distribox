using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Distribox.CommonLib;
using System.Diagnostics;

namespace Distribox.FileSystem
{
    /// <summary>
    /// Watcher of file system.
    /// </summary>
    public class FileWatcher
    {
        /// <summary>
        /// Occurs when file system event occurs.
        /// </summary>
        public delegate void FileSystemChangedHandler(FileChangedEventArgs e);

        /// <summary>
        /// Occurs when file changed.
        /// </summary>
        public event FileSystemChangedHandler Changed;

        /// <summary>
        /// Occurs when file created.
        /// </summary>
        public event FileSystemChangedHandler Created;

        /// <summary>
        /// Occurs when file renamed.
        /// </summary>
        public event FileSystemChangedHandler Renamed;

        /// <summary>
        /// Occurs when file deleted.
        /// </summary>
        public event FileSystemChangedHandler Deleted;

        /// <summary>
        /// Occurs when firstly idle from busy.
        /// </summary>
        public delegate void IdleHandler();

        /// <summary>
        /// Occurs when firstly idle from busy.
        /// </summary>
        public event IdleHandler Idle;

        /// <summary>
        /// The event queue.
        /// </summary>
        private Queue<FileSystemEventArgs> _eventQueue = new Queue<FileSystemEventArgs>();

        /// <summary>
        /// The timer for polling.
        /// </summary>
        private System.Timers.Timer timer = new System.Timers.Timer(Config.GetConfig().FileWatcherTimeIntervalMs);

        private string _dataPath
        {
            get { return Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep; }
        }

        private DateTime _lastEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.FileSystem.FileWatcher"/> class.
        /// </summary>
        /// <param name="root">Root.</param>
        public FileWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Config.GetConfig().RootFolder;
            watcher.Changed += OnWatcherEvent;
            watcher.Created += OnWatcherEvent;
            watcher.Renamed += OnWatcherEvent;
            watcher.Deleted += OnWatcherEvent;
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;

            timer.Elapsed += OnTimerEvent;
            timer.AutoReset = true;
            timer.Start();
        }

        /// <summary>
        /// Handle the timer event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_eventQueue)
            {
                if (_eventQueue.Count() == 0)
                    return;
            }
            timer.Stop();
            while (true)
            {
                FileSystemEventArgs args = null;
                lock (_eventQueue)
                {
                    if (_eventQueue.Count() == 0)
                        break;
                    args = _eventQueue.Dequeue();
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
                        Logger.Error("Unknown type of file watcher event: {0}.", args.Serialize());
                        break;
                }
            }
            Idle();
            timer.Start();
        }

        /// <summary>
        /// Handle the watcher event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnWatcherEvent(object sender, FileSystemEventArgs e)
        {
            // Exclude .Distribox folder
            if (e.Name.StartsWith(Properties.MetaFolder))
                return;
            lock (_eventQueue)
            {
                _eventQueue.Enqueue(e);
            }
        }

        /// <summary>
        /// Handle delete event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnDeletedEvent(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Deleted: {0}", e.Name);

            FileChangedEventArgs newEvent = TranslateEvent(e);
            if (Deleted != null)
                Deleted(newEvent);
        }

        /// <summary>
        /// Handle rename event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnRenamedEvent(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("Renamed: {0} -> {1}", e.OldName, e.Name);

            FileChangedEventArgs newEvent = TranslateEvent(e);
            
            if (!newEvent.IsDirectory)
            {
                // TODO remove sha1
                newEvent.SHA1 = CommonHelper.GetSHA1Hash(e.FullPath);
                newEvent.DataPath = _dataPath + newEvent.SHA1;
                if (!File.Exists(newEvent.DataPath))
                {
                    File.Copy(newEvent.FullPath, newEvent.DataPath);
                }
            }

            if (Renamed != null)
                Renamed(newEvent);
        }

        // TODO remove count
        int count = 0;
        /// <summary>
        /// Handle create event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnCreatedEvent(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Create: {0}  {1}", e.Name, count++);

            FileChangedEventArgs newEvent = TranslateEvent(e);

            if (Created != null)
                Created(newEvent);
        }

        /// <summary>
        /// Handle the changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnChangedEvent(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Changed: {0}", e.Name);
            FileChangedEventArgs newEvent = TranslateEvent(e);

            if (!newEvent.IsDirectory)
            {
                newEvent.SHA1 = CommonHelper.GetSHA1Hash(e.FullPath);
                newEvent.DataPath = _dataPath + newEvent.SHA1;
                if (!File.Exists(newEvent.DataPath))
                {
                    File.Copy(newEvent.FullPath, newEvent.DataPath);
                }
            }

            if (Changed != null)
                Changed(newEvent);
        }

        /// <summary>
        /// Translates the system event to FileChangedEvent.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="e">E.</param>
        private FileChangedEventArgs TranslateEvent(FileSystemEventArgs e)
        {
            FileChangedEventArgs newEvent = new FileChangedEventArgs();
            newEvent.ChangeType = e.ChangeType;
            newEvent.FullPath = e.FullPath;
            newEvent.Name = e.Name;

            if (_lastEvent.Ticks >= DateTime.Now.Ticks)
            {
                newEvent.When = _lastEvent.AddTicks(1);
            }
            else
            {
                newEvent.When = DateTime.Now;
            }
            _lastEvent = newEvent.When;

            if (e.ChangeType != WatcherChangeTypes.Deleted)
            {
                FileInfo info = new FileInfo(newEvent.FullPath);
                newEvent.IsDirectory = (info.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
            }
            if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                newEvent.OldName = ((RenamedEventArgs)e).OldName;
                newEvent.OldFullPath = ((RenamedEventArgs)e).OldFullPath;
            }
            return newEvent;
        }
    }
}
