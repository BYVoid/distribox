//-----------------------------------------------------------------------
// <copyright file="FileWatcher.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Distribox.CommonLib;

    /// <summary>
    /// Watcher of file system.
    /// </summary>
    public class FileWatcher : IDisposable
    {
        /// <summary>
        /// The timer for polling.
        /// </summary>
        private System.Timers.Timer timer = new System.Timers.Timer(Config.FileWatcherTimeIntervalMs);

        /// <summary>
        /// The event queue.
        /// </summary>
        private Queue<FileSystemEventArgs> eventQueue = new Queue<FileSystemEventArgs>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.FileSystem.FileWatcher"/> class.
        /// </summary>
        public FileWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Config.RootFolder.ToString();
            watcher.Changed += this.OnWatcherEvent;
            watcher.Created += this.OnWatcherEvent;
            watcher.Renamed += this.OnWatcherEvent;
            watcher.Deleted += this.OnWatcherEvent;
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;

            this.timer.Elapsed += this.OnTimerEvent;
            this.timer.AutoReset = true;
            this.timer.Start();
        }

        /// <summary>
        /// Occurs when file system event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        public delegate void FileSystemChangedHandler(FileChangedEventArgs e);

        /// <summary>
        /// Occurs when firstly idle from busy.
        /// </summary>
        public delegate void IdleHandler();

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
        public event IdleHandler Idle;
    
        /// <summary>
        /// Gets or sets the last event.
        /// </summary>
        /// <value>The last event.</value>
        private DateTime LastEvent { get; set; }

        /// <summary>
        /// Handle the timer event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (this.eventQueue)
            {
                if (this.eventQueue.Count() == 0)
                {
                    return;
                }
            }

            this.timer.Stop();
            while (true)
            {
                FileSystemEventArgs args = null;
                lock (this.eventQueue)
                {
                    if (this.eventQueue.Count() == 0)
                    {
                        break;
                    }

                    args = this.eventQueue.Dequeue();
                }

                switch (args.ChangeType)
                {
                    case WatcherChangeTypes.Changed:
                        this.OnChangedEvent(null, args);
                        break;
                    case WatcherChangeTypes.Created:
                        this.OnCreatedEvent(null, args);
                        break;
                    case WatcherChangeTypes.Deleted:
                        this.OnDeletedEvent(null, args);
                        break;
                    case WatcherChangeTypes.Renamed:
                        this.OnRenamedEvent(null, args as RenamedEventArgs);
                        break;
                    default:
                        Logger.Error("Unknown type of file watcher event: {0}.", args.Serialize());
                        break;
                }
            }

            this.Idle();
            this.timer.Start();
        }

        /// <summary>
        /// Handle the watcher event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnWatcherEvent(object sender, FileSystemEventArgs e)
        {
            // Display versions
            if (!GlobalFlag.AcceptFileEvent)
            {
                return;
            }

            // Exclude .Distribox folder
            if (e.Name.StartsWith(Properties.MetaFolder.ToString()))
            {
                return;
            }

            lock (this.eventQueue)
            {
                this.eventQueue.Enqueue(e);
            }
        }

        /// <summary>
        /// Handle delete event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnDeletedEvent(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Deleted: {0}", e.Name);

            FileChangedEventArgs newEvent = this.TranslateEvent(e);
            if (this.Deleted != null)
            {
                this.Deleted(newEvent);
            }
        }

        /// <summary>
        /// Handle rename event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnRenamedEvent(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("Renamed: {0} -> {1}", e.OldName, e.Name);

            FileChangedEventArgs newEvent = this.TranslateEvent(e);
            
            if (!newEvent.IsDirectory)
            {
                // TODO remove sha1
                newEvent.SHA1 = CommonHelper.GetSHA1Hash(e.FullPath);
                newEvent.DataPath = Config.MetaFolderData.File(newEvent.SHA1);
                if (!File.Exists(newEvent.DataPath))
                {
                    File.Copy(newEvent.FullPath, newEvent.DataPath);
                }
            }

            if (this.Renamed != null)
            {
                this.Renamed(newEvent);
            }
        }

        /// <summary>
        /// Handle create event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnCreatedEvent(object sender, FileSystemEventArgs e)
        {
            FileChangedEventArgs newEvent = this.TranslateEvent(e);

            if (this.Created != null)
            {
                this.Created(newEvent);
            }
        }

        /// <summary>
        /// Handle the changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnChangedEvent(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Changed: {0}", e.Name);
            FileChangedEventArgs newEvent = this.TranslateEvent(e);

            if (!newEvent.IsDirectory)
            {
                newEvent.SHA1 = CommonHelper.GetSHA1Hash(e.FullPath);
                newEvent.DataPath = Config.MetaFolderData.File(newEvent.SHA1);
                if (!File.Exists(newEvent.DataPath))
                {
                    File.Copy(newEvent.FullPath, newEvent.DataPath);
                }
            }

            if (this.Changed != null)
            {
                this.Changed(newEvent);
            }
        }

        /// <summary>
        /// Translates the system event to FileChangedEvent.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="e">The event args.</param>
        private FileChangedEventArgs TranslateEvent(FileSystemEventArgs e)
        {
            FileChangedEventArgs newEvent = new FileChangedEventArgs();
            newEvent.ChangeType = e.ChangeType;
            newEvent.FullPath = e.FullPath.Replace("\\", Properties.PathSep).Replace("/", Properties.PathSep);
            newEvent.Name = e.Name.Replace("\\", Properties.PathSep).Replace("/", Properties.PathSep);

            if (this.LastEvent.Ticks >= DateTime.Now.Ticks)
            {
                newEvent.When = this.LastEvent.AddTicks(1);
            }
            else
            {
                newEvent.When = DateTime.Now;
            }

            this.LastEvent = newEvent.When;

            if (e.ChangeType != WatcherChangeTypes.Deleted)
            {
                FileInfo info = new FileInfo(newEvent.FullPath);
                newEvent.IsDirectory = (info.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
            }

            if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                newEvent.OldName = ((RenamedEventArgs)e).OldName.Replace("\\", Properties.PathSep).Replace("/", Properties.PathSep);
                newEvent.OldFullPath = ((RenamedEventArgs)e).OldFullPath.Replace("\\", Properties.PathSep).Replace("/", Properties.PathSep);
            }

            return newEvent;
        }

		/// <summary>
		/// Releases all resource used by the <see cref="Distribox.FileSystem.FileWatcher"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Distribox.FileSystem.FileWatcher"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Distribox.FileSystem.FileWatcher"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the
		/// <see cref="Distribox.FileSystem.FileWatcher"/> so the garbage collector can reclaim the memory that the
		/// <see cref="Distribox.FileSystem.FileWatcher"/> was occupying.</remarks>
        public void Dispose()
        {
            this.timer.Dispose();
        }
    }
}
