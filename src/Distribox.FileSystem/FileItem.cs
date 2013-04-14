//-----------------------------------------------------------------------
// <copyright file="FileItem.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
        /// Initializes a new instance of the <see cref="Distribox.FileSystem.FileItem"/> class.
        /// </summary>
        public FileItem()
        {
            this.History = new List<FileEvent>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.FileSystem.FileItem"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public FileItem(string id)
        {
            this.History = new List<FileEvent>();
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.FileSystem.FileItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isDirectory">If set to <c>true</c> is directory.</param>
        public FileItem(string name, bool isDirectory)
        {
            this.History = new List<FileEvent>();
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
                return this.History.Last().Type != FileEventType.Deleted;
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
                return this.History.Count() == 0 ? null : this.History.Last().Name;
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
                return this.History.Count() == 0 ? null : this.History.Last().SHA1;
            }
        }

        /// <summary>
        /// Gets current file size.
        /// </summary>
        /// <value>File size.</value>
        public long CurrentSize
        {
            get
            {
                return this.History.Count() == 0 ? 0 : this.History.Last().Size;
            }
        }

        /// <summary>
        /// Gets current parent id.
        /// </summary>
        /// <value>Current parent id.</value>
        public string CurrentParentId
        {
            get
            {
                return this.History.Count() == 0 ? null : this.History.Last().EventId;
            }
        }

        /// <summary>
        /// Gets or sets history of the file.
        /// </summary>
        /// <value>The history of the file.</value>
        public List<FileEvent> History { get; set; }

        /// <summary>
        /// Create the initial version.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="when">Then when.</param>
        public void Create(string name, DateTime when)
        {
            FileEvent vs = new FileEvent();
            vs.FileId = this.Id;
            vs.EventId = CommonHelper.GetRandomHash();
            vs.ParentId = this.CurrentParentId;
            vs.IsDirectory = this.IsDirectory;
            vs.Name = name;
            vs.When = when;
            vs.SHA1 = null;
            vs.Size = 0;
            vs.Type = FileEventType.Created;

            this.PushHistory(vs);
        }

        /// <summary>
        /// Create a version of renaming
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="when">The when.</param>
        public void Rename(string name, DateTime when)
        {
            FileEvent vs = new FileEvent();
            vs.FileId = this.Id;
            vs.EventId = CommonHelper.GetRandomHash();
            vs.ParentId = this.CurrentParentId;
            vs.IsDirectory = this.IsDirectory;
            vs.Name = name;
            vs.When = when;
            vs.SHA1 = this.CurrentSHA1;
            vs.Size = this.CurrentSize;
            vs.Type = FileEventType.Renamed;

            this.PushHistory(vs);
        }

        /// <summary>
        /// Create a version of deleting
        /// </summary>
        /// <param name="when">The when.</param>
        public void Delete(DateTime when)
        {
            FileEvent vs = new FileEvent();
            vs.FileId = this.Id;
            vs.EventId = CommonHelper.GetRandomHash();
            vs.ParentId = this.CurrentParentId;
            vs.IsDirectory = this.IsDirectory;
            vs.Name = this.CurrentName;
            vs.When = when;
            vs.SHA1 = this.CurrentSHA1;
            vs.Size = this.CurrentSize;
            vs.Type = FileEventType.Deleted;

            this.PushHistory(vs);
        }

        /// <summary>
        /// Create a version of changing.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sha1">The SHA1.</param>
        /// <param name="when">The when.</param>
        public void Change(string name, string sha1, DateTime when)
        {
            FileEvent vs = new FileEvent();
            vs.FileId = this.Id;
            vs.EventId = CommonHelper.GetRandomHash();
            vs.ParentId = this.CurrentParentId;
            vs.IsDirectory = this.IsDirectory;
            vs.Name = this.CurrentName;
            vs.When = when;
            vs.SHA1 = sha1;
            vs.Size = this.IsDirectory || sha1 == null ? 0 : (new FileInfo(Config.MetaFolderData.File(vs.SHA1))).Length;
            vs.Type = FileEventType.Changed;

            this.PushHistory(vs);
        }

        /// <summary>
        /// Merge the specified event.
        /// </summary>
        /// <param name="vs">The event.</param>
        public void Merge(FileEvent vs)
        {
            if (this.History.Count() == 0)
            {
                this.Id = vs.FileId;
                this.IsDirectory = vs.IsDirectory;

                this.PushHistory(vs);
                if (this.IsDirectory)
                {
                    switch (vs.Type)
                    {
                        case FileEventType.Created:
                            GlobalFlag.AcceptFileEvent = false;
                            Directory.CreateDirectory(Config.RootFolder.Enter(vs.Name));
                            GlobalFlag.AcceptFileEvent = true;
                            break;
                        default:
                            Debug.Assert(false, "Not implement!");
                            break;
                    }
                }
                else
                {
                    switch (vs.Type)
                    {
                        case FileEventType.Created:
                            if (vs.SHA1 == null)
                            {
                                GlobalFlag.AcceptFileEvent = false;
                                File.WriteAllText(Config.RootFolder.File(vs.Name), string.Empty);
                                GlobalFlag.AcceptFileEvent = true;
                            }
                            else
                            {
                                GlobalFlag.AcceptFileEvent = false;
                                File.Copy(Config.MetaFolderData.File(vs.SHA1), Config.RootFolder.File(vs.Name));
                                GlobalFlag.AcceptFileEvent = true;
                            }

                            break;
                        default:
                            Debug.Assert(false, "Not implement!");
                            break;
                    }
                }

                return;
            }

            FileEvent last = this.History.Last();
            this.PushHistory(vs);

            // Display changes
            if (vs.When.Ticks > last.When.Ticks)
            {
                if (this.IsDirectory)
                {
                    switch (vs.Type)
                    {
                        case FileEventType.Created:
                            GlobalFlag.AcceptFileEvent = false;
                            Directory.CreateDirectory(Config.RootFolder.Enter(vs.Name));
                                GlobalFlag.AcceptFileEvent = true;
                            break;
                        case FileEventType.Changed:
                            break;
                        case FileEventType.Renamed:
                            GlobalFlag.AcceptFileEvent = false;
                            Directory.Move(Config.RootFolder.Enter(last.Name), Config.RootFolder.Enter(vs.Name));
                            GlobalFlag.AcceptFileEvent = true;
                            break;
                        case FileEventType.Deleted:
                            GlobalFlag.AcceptFileEvent = false;
                            Directory.Delete(Config.RootFolder.Enter(vs.Name));
                            GlobalFlag.AcceptFileEvent = true;
                            break;
                    }
                }
                else
                {
                    switch (vs.Type)
                    {
                        case FileEventType.Created:
                            if (vs.SHA1 == null)
                            {
                                GlobalFlag.AcceptFileEvent = false;
                                File.WriteAllText(Config.RootFolder.File(vs.Name), string.Empty);
                                GlobalFlag.AcceptFileEvent = true;
                            }
                            else
                            {
                                GlobalFlag.AcceptFileEvent = false;
                                File.Copy(Config.MetaFolderData.File(vs.SHA1), Config.RootFolder.File(vs.Name));
                                GlobalFlag.AcceptFileEvent = true;
                            }

                            break;
                        case FileEventType.Changed:
                            if (vs.SHA1 == null)
                            {
                                GlobalFlag.AcceptFileEvent = false;
                                File.WriteAllText(Config.RootFolder.File(vs.Name), string.Empty);
                                GlobalFlag.AcceptFileEvent = true;
                            }
                            else
                            {
                                GlobalFlag.AcceptFileEvent = false;
                                File.Copy(Config.MetaFolderData.File(vs.SHA1), Config.RootFolder.File(vs.Name), true);
                                GlobalFlag.AcceptFileEvent = true;
                            }

                            break;
                        case FileEventType.Renamed:
                            GlobalFlag.AcceptFileEvent = false;
                            File.Move(Config.RootFolder.File(last.Name), Config.RootFolder.File(vs.Name));
                            GlobalFlag.AcceptFileEvent = true;
                            break;
                        case FileEventType.Deleted:
                            GlobalFlag.AcceptFileEvent = false;
                            File.Delete(Config.RootFolder.File(vs.Name));
                            GlobalFlag.AcceptFileEvent = true;
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Push the history.
        /// </summary>
        /// <param name="vs">The event.</param>
        private void PushHistory(FileEvent vs)
        {
            this.History.Add(vs);
            this.History.Sort((x, y) => x.When.Ticks.CompareTo(y.When.Ticks));
        }
    }
}
