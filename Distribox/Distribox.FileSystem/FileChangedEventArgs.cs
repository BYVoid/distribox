namespace Distribox.FileSystem
{
    using System;
    using System.IO;

    /// <summary>
    /// Arguments of file changing event.
    /// </summary>
    public class FileChangedEventArgs
    {
        public WatcherChangeTypes ChangeType { get; set; }

        public string FullPath { get; set; }

        public string Name { get; set; }

        public string OldFullPath { get; set; }

        public string OldName { get; set; }

        public string SHA1 { get; set; }

        public string DataPath { get; set; }

        public DateTime When { get; set; }

        public bool IsDirectory { get; set; }
    }
}
