//-----------------------------------------------------------------------
// <copyright file="VersionList.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Distribox.CommonLib;    

    // TODO this class would be serialized for deserialized

    // this class forbid serialize

    /// <summary>
    /// List of all versions.
    /// </summary>
    public class VersionList
    {
        /// <summary>
        /// Maps relative path to file item object
        /// </summary>
        private Dictionary<string, FileItem> pathToFile = new Dictionary<string, FileItem>();

        /// <summary>
        /// The path of version list.
        /// </summary>
        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.FileSystem.VersionList"/> class.
        /// </summary>
        public VersionList()
        {
            this.path = Config.VersionListFilePath;

            // Deserialize version list
            this.AllFiles = CommonHelper.ReadObject<List<FileItem>>(this.path);
            foreach (var file in this.AllFiles.Where(x => x.IsAlive))
            {
                this.pathToFile[file.CurrentName] = file;
            }
        }

        /// <summary>
        /// Gets or sets all files that ever existed.
        /// </summary>
        /// <value>All files.</value>
        public List<FileItem> AllFiles { get; set; }

        /// <summary>
        /// Create a file item
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isDirectory">If set to <c>true</c> is directory.</param>
        /// <param name="when">The when.</param>
        /// <returns>The file item.</returns>
        public FileItem Create(string name, bool isDirectory, DateTime when)
        {
            if (this.pathToFile.ContainsKey(name))
            {
                return null;
            }

            FileItem item = new FileItem(name, isDirectory);
            item.Create(name, when);

            this.AllFiles.Add(item);

            this.pathToFile.Add(name, item);

            return item;
        }

        /// <summary>
        /// Change a file item.
        /// </summary>
        /// <param name="name">The Name.</param>
        /// <param name="isDirectory">If set to <c>true</c> is directory.</param>
        /// <param name="sha1">The SHA1.</param>
        /// <param name="when">The when.</param>
        public void Change(string name, bool isDirectory, string sha1, DateTime when)
        {
            if (!this.pathToFile.ContainsKey(name))
            {
                this.Create(name, isDirectory, when);
            }

            if (isDirectory)
            {
                return;
            }

            FileItem item = this.pathToFile[name];
            item.Change(name, sha1, when);
        }

        /// <summary>
        /// Rename a file item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="oldName">Old name.</param>
        /// <param name="sha1">The SHA1.</param>
        /// <param name="when">The when.</param>
        public void Rename(string name, string oldName, string sha1, DateTime when)
        {
            FileItem item = this.pathToFile[oldName];

            item.Rename(name, when);

            this.pathToFile.Remove(oldName);
            this.pathToFile[name] = item;
        }

        /// <summary>
        /// Delete a file item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="when">The when.</param>
        public void Delete(string name, DateTime when)
        {
            FileItem item = this.pathToFile[name];
            item.Delete(when);
            this.pathToFile.Remove(name);
        }

        /// <summary>
        /// Gets all version that exist in <paramref name="list"/> but not exist in this
        /// </summary>
        /// <returns>The less than.</returns>
        /// <param name="list">The list.</param>
        public List<FileEvent> GetLessThan(VersionList list)
        {
            HashSet<string> myFileList = new HashSet<string>();
            foreach (var item in this.AllFiles)
            {
                foreach (var history in item.History)
                {
                    myFileList.Add(item.Id + "@" + history.Serialize());
                }
            }

            List<FileEvent> patches = new List<FileEvent>();
            foreach (var item in list.AllFiles)
            {
                foreach (var history in item.History)
                {
                    string guid = item.Id + "@" + history.Serialize();
                    if (myFileList.Contains(guid))
                    {
                        continue;
                    }

                    patches.Add(history);
                }
            }

            return patches;
        }

        /// <summary>
        /// Flush version list into disk
        /// </summary>
        public void Flush()
        {
            this.AllFiles.WriteObject(this.path);
        }

        /// <summary>
        /// Gets file by name.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="name">The name.</param>
        public FileItem GetFileByName(string name)
        {
            return this.pathToFile[name];
        }

        /// <summary>
        /// Gets file by Id.
        /// </summary>
        /// <param name="fileId">The file ID.</param>
        /// <returns>The file item.</returns>
        public FileItem GetFileById(string fileId)
        {
            foreach (var item in this.AllFiles)
            {
                if (item.Id == fileId)
                {
                    return item;
                }
            }

            throw new Exception("Not Find File!");
        }

        /// <summary>
        /// Sets file by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fileItem">File item.</param>
        public void SetFileByName(string name, FileItem fileItem)
        {
            this.pathToFile[name] = fileItem;
        }

        /// <summary>
        /// Removes the name of the file by.
        /// </summary>
        /// <param name="oldName">Old name.</param>
        public void RemoveFileByName(string oldName)
        {
            if (oldName == null)
            {
                return;
            }

            this.pathToFile.Remove(oldName);
        }
    }
}
