using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.FileSystem
{
	/// <summary>
	/// File subversion type.
	/// </summary>
    public enum FileSubversionType
    {
        Created, Changed, Renamed, Deleted
    }

	/// <summary>
	/// File subversion.
	/// </summary>
    public class FileSubversion
    {
        public string Name { get; set; }
		public string SHA1 { get; set; }
        public DateTime LastModify { get; set; }
        public FileSubversionType Type { get; set; }
    }
}
