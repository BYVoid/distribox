using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.FileSystem
{
	/// <summary>
	/// File subversion type.
	/// </summary>
    public enum FileEventType
    {
        Created, Changed, Renamed, Deleted
    }

	/// <summary>
	/// File Event.
	/// </summary>
    public class FileEvent
    {
        public string fileId { get; set; }
        public string eventId { get; set; }

        public string ParentId { get; set; }

        public bool IsDirectory { get; set; }

        public string Name { get; set; }
        public string SHA1 { get; set; }

        public DateTime LastModify { get; set; }
        public FileEventType Type { get; set; }

        public long Size { get; set; }
    }
}
