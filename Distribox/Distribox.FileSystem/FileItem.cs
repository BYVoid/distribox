using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distribox.CommonLib;
using System.Collections;
using System.IO;

namespace Distribox.FileSystem
{
    // TODO create a new class indicating version delta
    /// <summary>
    /// Stores all versions of a file.
    /// </summary>
    public class FileItem
    {
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
        /// Gets or sets a value indicating whether this file is alive (not deleted).
        /// </summary>
        /// <value><c>true</c> if this file is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get
            {
                return History.Last().Value.Type != FileEventType.Deleted;
            }
        }

        /// <summary>
        /// Gets or sets current name of the file.
        /// </summary>
        /// <value>The current name of the file.</value>
        public string CurrentName
        {
            get
            {
                return History.Last().Value.Name;
            }
        }

        /// <summary>
        /// Gets or sets SHA1 checksum.
        /// </summary>
        /// <value>SHA1 chcksum.</value>
        public string CurrentSHA1
        {
            get
            {
                return History.Last().Value.SHA1;
            }
        }

        /// <summary>
        /// Gets or sets history of the file.
        /// </summary>
        /// <value>The history of the file.</value>
        public SortedList<long, FileEvent> History { get; set; }

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
        /// <param name="Id">Identifier.</param>
        public FileItem(string Id)
        {
            this.History = new SortedList<long, FileEvent>();
            this.Id = Id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.FileItem"/> class.
        /// </summary>
        /// <param name="Name">Name.</param>
        /// <param name="IsDirectory">If set to <c>true</c> is directory.</param>
        public FileItem(string Name, bool IsDirectory)
        {
            this.History = new SortedList<long, FileEvent>();
            this.Id = CommonHelper.GetRandomHash();
            this.IsDirectory = IsDirectory;
        }

        /// <summary>
        /// Create the initial version.
        /// </summary>
        /// <param name="When">When.</param>
        public void Create(String Name, DateTime When)
        {
            FileEvent vs = new FileEvent();
            vs.Type = FileEventType.Created;
            vs.Name = Name;
            vs.LastModify = When;
            vs.SHA1 = null;
            vs.Size = 0;

            History.Add(vs.LastModify.Ticks, vs);
        }

        /// <summary>
        /// Create a version of renaming
        /// </summary>
        /// <param name="Name">Name.</param>
        /// <param name="When">When.</param>
        public void Rename(string Name, DateTime When)
        {
            if (Name == CurrentName)
                return;

            FileEvent vs = new FileEvent();
            vs.Type = FileEventType.Renamed;
            vs.Name = Name;
            vs.LastModify = When;
            vs.SHA1 = CurrentSHA1;
            if (vs.SHA1 != null)
            {
                vs.Size = (new FileInfo(Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + vs.SHA1)).Length;
            }
            else
            {
                vs.Size = 0;
            }

            History.Add(vs.LastModify.Ticks, vs);
        }

        /// <summary>
        /// Create a version of deleting
        /// </summary>
        /// <param name="When">When.</param>
        public void Delete(DateTime When)
        {
            FileEvent vs = new FileEvent();
            vs.Type = FileEventType.Deleted;
            vs.Name = CurrentName;
            vs.LastModify = When;
            vs.SHA1 = CurrentSHA1;
            if (vs.SHA1 != null)
            {
                vs.Size = (new FileInfo(Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + vs.SHA1)).Length;
            }
            else
            {
                vs.Size = 0;
            }

            History.Add(vs.LastModify.Ticks, vs);
        }

        /// <summary>
        /// Create a version of changing
        /// </summary>
        /// <param name="Name">Name.</param>
        /// <param name="SHA1">SH a1.</param>
        /// <param name="When">When.</param>
        public void Change(string Name, string SHA1, DateTime When)
        {
            if (SHA1 == CurrentSHA1)
            {
                return;
            }

            FileEvent vs = new FileEvent();
            vs.Type = FileEventType.Changed;
            vs.Name = Name;
            vs.LastModify = When;
            vs.SHA1 = SHA1;
            if (vs.SHA1 != null)
            {
                FileInfo info = new FileInfo(Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + vs.SHA1);
                vs.Size = info.Length;
            }
            else
            {
                vs.Size = 0;
            }

            Console.WriteLine(History.SerializeInline());
            Console.WriteLine(vs.SerializeInline());
            History.Add(vs.LastModify.Ticks, vs);
        }

        /// <summary>
        /// Apply patch.
        /// </summary>
        /// <param name="patch">patch.</param>
        public void ApplyPatch(String root, AtomicPatch patch)
        {
            if (History.ContainsKey(patch.LastModify.Ticks)) return;
            
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
            if (History.Count() == 0)
            {
                if (IsDirectory)
                {
                    Directory.CreateDirectory(root + CurrentName);
                }
                else
                {
                    if (patch.SHA1 == null)
                    {
                        File.WriteAllText(root + vs.Name, "");
                    }
                    else
                    {
                        File.Copy(root + ".Distribox/data/" + patch.SHA1, root + vs.Name, true);
                    }
                }
                History.Add(vs.LastModify.Ticks, vs);
                return;
            }

            // Change file or directory.
            FileEvent lastVersion = History.Last().Value;
            History.Add(vs.LastModify.Ticks, vs);
            if (lastVersion.LastModify.Ticks != patch.LastModify.Ticks)
            {
                if (IsDirectory)
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
                        File.WriteAllText(root + vs.Name, "");
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
