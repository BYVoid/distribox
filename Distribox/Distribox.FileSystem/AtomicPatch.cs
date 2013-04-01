using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.FileSystem
{
    /// <summary>
    /// Meta data of an atomic patch. An atomic patch is the difference between two versions of file. 
    /// </summary>
    /// <remarks>
    /// This class is used for identifing a patch request by <see cref="Distribox.Network.AntiEntropyProtocol>.
    /// </remarks>
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Distribox.FileSystem.AtomicPatch"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Distribox.FileSystem.AtomicPatch"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Distribox.FileSystem.AtomicPatch"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var patch = (AtomicPatch)obj;
            return String.Format("{0}@{1}", Id, LastModify.Ticks) == String.Format("{0}@{1}", patch.Id, patch.LastModify.Ticks);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Distribox.FileSystem.AtomicPatch"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return String.Format("{0}@{1}", Id, LastModify.Ticks).GetHashCode();
        }
    }
}
