using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public enum FileSubversionType
    {
        Created, Changed, Renamed, Deleted
    }

    public class FileSubversion
    {
        public string Name { get; set; }
		public string SHA1 { get; set; }
        public DateTime LastModify { get; set; }
        public FileSubversionType Type { get; set; }
    }
}
