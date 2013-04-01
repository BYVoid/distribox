namespace Distribox.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Distribox.CommonLib;

    // TODO create a new class indicating version delta

    /// <summary>
    /// Stores all versions of a file.
    /// </summary>
    public class FileItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.FileItem"/> class.
        /// This constructor is only used for serialization
        /// </summary>
        public FileItem()
        {
            this.History = new SortedList<long, FileEvent>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.FileItem"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public FileItem(string id)
        {
            this.History = new SortedList<long, FileEvent>();
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.FileItem"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="isDirectory">If set to <c>true</c> is directory.</param>
        public FileItem(string name, bool isDirectory)
        {
            this.History = new SortedList<long, FileEvent>();
            this.Id = CommonHelper.GetRandomHash();
            this.IsDirectory = isDirectory;
        }

        /// <summary>
        /// Gets or sets the identifier of file.
        /// </summary>
        /// <value>The identifier of this file.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this file is a directory.
        /// </summary>
        /// <value><c>true</c> if this file is a directory; otherwise, <c>false</c>.</value>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// Gets a value indicating whether this file is alive (not deleted).
        /// </summary>
        /// <value><c>true</c> if this file is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get
            {
                return this.History.Last().Value.Type != FileEventType.Deleted;
            }
        }

        /// <summary>
        /// Gets current name of the file.
        /// </summary>
        /// <value>The current name of the file.</value>
        public string CurrentName
        {
            get
            {
                return this.History.Last().Value.Name;
            }
        }

        /// <summary>
        /// Gets SHA1 checksum.
        /// </summary>
        /// <value>SHA1 checksum.</value>
        public string CurrentSHA1
        {
            get
            {
                return this.History.Last().Value.SHA1;
            }
        }

        /// <summary>
        /// Gets or sets history of the file.
        /// </summary>
        /// <value>The history of the file.</value>
        public SortedList<long, FileEvent> History { get; set; }

        /// <summary>
        /// Create the initial version.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="when">Then when.</param>
        public void Create(string name, DateTime when)
        {
            FileEvent vs = new FileEvent();
            vs.Type = FileEventType.Created;
            vs.Name = name;
            vs.LastModify = when;
            vs.SHA1 = null;
            vs.Size = 0;

            this.History.Add(vs.LastModify.Ticks, vs);
        }

        /// <summary>
        /// Create a version of renaming
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="when">When.</param>
        public void Rename(string name, DateTime when)
        {
            if (name == this.CurrentName)
            {
                return;
            }

            FileEvent vs = new FileEvent();
            vs.Type = FileEventType.Renamed;
            vs.Name = name;
            vs.LastModify = when;
            vs.SHA1 = this.CurrentSHA1;
            if (vs.SHA1 != null)
            {
                vs.Size = (new FileInfo(Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + vs.SHA1)).Length;
            }
            else
            {
                vs.Size = 0;
            }

            this.History.Add(vs.LastModify.Ticks, vs);
        }

        /// <summary>
        /// Create a version of deleting
        /// </summary>
        /// <param name="when">When.</param>
        public void Delete(DateTime when)
        {
            FileEvent vs = new FileEvent();
            vs.Type = FileEventType.Deleted;
            vs.Name = this.CurrentName;
            vs.LastModify = when;
            vs.SHA1 = this.CurrentSHA1;
            if (vs.SHA1 != null)
            {
                vs.Size = (new FileInfo(Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + vs.SHA1)).Length;
            }
            else
            {
                vs.Size = 0;
            }

            this.History.Add(vs.LastModify.Ticks, vs);
        }

        /// <summary>
        /// Create a version of changing
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="sha1">SH a1.</param>
        /// <param name="when">When.</param>
        public void Change(string name, string sha1, DateTime when)
        {
            if (sha1 == this.CurrentSHA1)
            {
                return;
            }

            FileEvent vs = new FileEvent();
            vs.Type = FileEventType.Changed;
            vs.Name = name;
            vs.LastModify = when;
            vs.SHA1 = sha1;
            if (vs.SHA1 != null)
            {
                FileInfo info = new FileInfo(Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + vs.SHA1);
                vs.Size = info.Length;
            }
            else
            {
                vs.Size = 0;
            }

            Console.WriteLine(this.History.SerializeInline());
            Console.WriteLine(vs.SerializeInline());
            this.History.Add(vs.LastModify.Ticks, vs);
        }

        /// <summary>
        /// Apply patch.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="patch">The patch.</param>
        public void ApplyPatch(string root, AtomicPatch patch)
        {
            if (this.History.ContainsKey(patch.LastModify.Ticks))
            {
                return;
            }
            
            this.IsDirectory = patch.IsDirectory;
            
            FileEvent vs = new FileEvent();
            vs.Type = patch.Type;
            vs.Name = patch.Name;
            vs.LastModify = patch.LastModify;
            vs.SHA1 = patch.SHA1;
            if (vs.SHA1 != null)
            {
                vs.Size = (new FileInfo(Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + vs.SHA1)).Length;
            }
            else
            {
                vs.Size = 0;
            }

            // Receive new file
            if (this.History.Count() == 0)
            {
                if (this.IsDirectory)
                {
                    Directory.CreateDirectory(root + this.CurrentName);
                }
                else
                {
                    if (patch.SHA1 == null)
                    {
                        File.WriteAllText(root + vs.Name, string.Empty);
                    }
                    else
                    {
                        File.Copy(root + ".Distribox/data/" + patch.SHA1, root + vs.Name, true);
                    }
                }

                this.History.Add(vs.LastModify.Ticks, vs);
                return;
            }

            // Change file or directory.
            FileEvent lastVersion = this.History.Last().Value;
            this.History.Add(vs.LastModify.Ticks, vs);
            if (lastVersion.LastModify.Ticks != patch.LastModify.Ticks)
            {
                if (this.IsDirectory)
                {
                    if (lastVersion.Name != patch.Name)
                    {
                        Directory.Delete(root + lastVersion.Name);
                        Directory.CreateDirectory(root + patch.Name);
                    }
                }
                else
                {
                    if (patch.SHA1 == null)
                    {
                        File.WriteAllText(root + vs.Name, string.Empty);
                    }
                    else
                    {
                        File.Copy(root + ".Distribox/data/" + patch.SHA1, root + vs.Name, true);
                    }
                }
            }
        }
    }
}
