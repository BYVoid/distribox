//-----------------------------------------------------------------------
// <copyright file="FileEvent.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.FileSystem
{
    using Distribox.CommonLib;

    /// <summary>
    /// File subversion type.
    /// </summary>
    public enum FileEventType
    {
        /// <summary>
        /// The created.
        /// </summary>
        Created,

        /// <summary>
        /// The changed.
        /// </summary>
        Changed,

        /// <summary>
        /// The renamed.
        /// </summary>
        Renamed,

        /// <summary>
        /// The deleted.
        /// </summary>
        Deleted
    }

    /// <summary>
    /// File Events.
    /// </summary>
    public class FileEvent
    {
        /// <summary>
        /// Gets or sets the file identifier.
        /// </summary>
        /// <value>The file identifier.</value>
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>The event identifier.</value>
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is directory.
        /// </summary>
        /// <value><c>true</c> if this instance is directory; otherwise, <c>false</c>.</value>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the when.
        /// </summary>
        /// <value>The when.</value>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets or sets the SH a1.
        /// </summary>
        /// <value>The SH a1.</value>
        public string SHA1 { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public FileEventType Type { get; set; }

        /// <summary>
        /// Creates the event.
        /// </summary>
        /// <returns>The event.</returns>
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
            return this.EventId.GetHashCode();
        }
    }
}
