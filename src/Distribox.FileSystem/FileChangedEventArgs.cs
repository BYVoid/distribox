//-----------------------------------------------------------------------
// <copyright file="FileChangedEventArgs.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.FileSystem
{
    using System;
    using System.IO;

    /// <summary>
    /// Arguments of file changing event.
    /// </summary>
    public class FileChangedEventArgs
    {
        /// <summary>
        /// Gets or sets the type of the change.
        /// </summary>
        /// <value>The type of the change.</value>
        public WatcherChangeTypes ChangeType { get; set; }

        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the old full path.
        /// </summary>
        /// <value>The old full path.</value>
        public string OldFullPath { get; set; }

        /// <summary>
        /// Gets or sets the old name.
        /// </summary>
        /// <value>The old name.</value>
        public string OldName { get; set; }

        /// <summary>
        /// Gets or sets the SH a1.
        /// </summary>
        /// <value>The SH a1.</value>
        public string SHA1 { get; set; }

        /// <summary>
        /// Gets or sets the data path.
        /// </summary>
        /// <value>The data path.</value>
        public string DataPath { get; set; }

        /// <summary>
        /// Gets or sets the when.
        /// </summary>
        /// <value>The when.</value>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is directory.
        /// </summary>
        /// <value><c>true</c> if this instance is directory; otherwise, <c>false</c>.</value>
        public bool IsDirectory { get; set; }
    }
}
