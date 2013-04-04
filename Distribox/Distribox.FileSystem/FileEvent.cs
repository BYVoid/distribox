namespace Distribox.FileSystem
{
    using Distribox.CommonLib;
    using System;

    /// <summary>
    /// File subversion type.
    /// </summary>
    public enum FileEventType
    {
        Created, Changed, Renamed, Deleted
    }

    /// <summary>
    /// File Events.
    /// </summary>
    public class FileEvent
    {
        public string FileId { get; set; }

        public string EventId { get; set; }

        public string ParentId { get; set; }

        public bool IsDirectory { get; set; }

        public string Name { get; set; }

        public DateTime When { get; set; }

        public string SHA1 { get; set; }

        public long Size { get; set; }

        public FileEventType Type { get; set; }

        public static FileEvent CreateEvent()
        {
            FileEvent e = new FileEvent();
            e.EventId = CommonHelper.GetRandomHash();
            return e;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Distribox.FileSystem.AtomicPatch"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Distribox.FileSystem.AtomicPatch"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Distribox.FileSystem.AtomicPatch"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return this.EventId == ((FileEvent)obj).EventId;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Distribox.FileSystem.AtomicPatch"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return EventId.GetHashCode();
        }
    }
}
