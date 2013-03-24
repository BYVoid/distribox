using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
	// TODO create a new class indicating version delta
	/// <summary>
	/// Stores all versions of a file.
	/// </summary>
    public class FileItem
    {
		/// <summary>
		/// Gets or sets a value indicating whether this file is alive (not deleted).
		/// </summary>
		/// <value><c>true</c> if this file is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive { get; set; }

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
		/// Gets or sets current name of the file.
		/// </summary>
		/// <value>The current name of the file.</value>
        public string CurrentName { get; set; }

		/// <summary>
		/// Gets or sets SHA1 checksum.
		/// </summary>
		/// <value>SHA1 chcksum.</value>
        public string CurrentSHA1 { get; set; }

		/// <summary>
		/// Gets or sets history of the file.
		/// </summary>
		/// <value>The history of the file.</value>
        public List<FileSubversion> History { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Distribox.CommonLib.FileItem"/> class.
		/// This constructor is only used for serialization
		/// </summary>
        public FileItem()
        {
            this.History = new List<FileSubversion>();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Distribox.CommonLib.FileItem"/> class.
		/// </summary>
		/// <param name="Id">Identifier.</param>
        public FileItem(string Id)
        {
            this.History = new List<FileSubversion>();
            this.Id = Id;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Distribox.CommonLib.FileItem"/> class.
		/// </summary>
		/// <param name="Name">Name.</param>
		/// <param name="IsDirectory">If set to <c>true</c> is directory.</param>
        public FileItem(string Name, bool IsDirectory)
        {
            this.History = new List<FileSubversion>();
            this.IsAlive = true;
            this.Id = CommonHelper.GetRandomHash();
            this.IsDirectory = IsDirectory;
            this.CurrentName = Name;
            this.CurrentSHA1 = null;
        }

		/// <summary>
		/// Create the initial version.
		/// </summary>
		/// <param name="When">When.</param>
        public void Create(DateTime When)
        {
            FileSubversion vs = new FileSubversion();
            vs.Type = FileSubversionType.Created;
            vs.Name = CurrentName;
            vs.LastModify = When;
            vs.SHA1 = null;

            CurrentSHA1 = null;
            History.Add(vs);
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

            FileSubversion vs = new FileSubversion();
            vs.Type = FileSubversionType.Renamed;
            vs.Name = Name;
            vs.LastModify = When;
            vs.SHA1 = CurrentSHA1;

            CurrentName = Name;
            History.Add(vs);
        }

		/// <summary>
		/// Create a version of deleting
		/// </summary>
		/// <param name="When">When.</param>
        public void Delete(DateTime When)
        {
            FileSubversion vs = new FileSubversion();
            vs.Type = FileSubversionType.Deleted;
            vs.Name = CurrentName;
            vs.LastModify = When;
            vs.SHA1 = CurrentSHA1;

            IsAlive = false;
            History.Add(vs);
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
				return;

            FileSubversion vs = new FileSubversion();
            vs.Type = FileSubversionType.Changed;
            vs.Name = Name;
            vs.LastModify = When;
            vs.SHA1 = SHA1;

            CurrentSHA1 = SHA1;
            History.Add(vs);
        }

		/// <summary>
		/// Insert a new version into history
		/// </summary>
		/// <param name="vs">Vs.</param>
        public void NewVersion(FileSubversion vs)
        {
			// TODO insert the version to correct position
            History.Add(vs);
        }
    }
}
