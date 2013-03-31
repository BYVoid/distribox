using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.FileSystem
{
    public class AtomicPatch
    {
        public string Id { get; set; }
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public string SHA1 { get; set; }
        public DateTime LastModify { get; set; }
        public FileEventType Type { get; set; }

        /// <summary>
        /// Size of the atomic patch, in bytes
        /// </summary>
        /// TODO (curimit: I add this property, I need this to compute request)
        public int Size { get; set; }
    }
}
