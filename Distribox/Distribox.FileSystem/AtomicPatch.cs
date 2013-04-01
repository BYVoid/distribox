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
        public long Size { get; set; }

        public override bool Equals(object obj)
        {
            var patch = (AtomicPatch)obj;
            return String.Format("{0}@{1}", Id, LastModify.Ticks) == String.Format("{0}@{1}", patch.Id, patch.LastModify.Ticks);
        }

        public override int GetHashCode()
        {
            return String.Format("{0}@{1}", Id, LastModify.Ticks).GetHashCode();
        }
    }
}
