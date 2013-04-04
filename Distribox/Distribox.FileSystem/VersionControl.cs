namespace Distribox.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Distribox.CommonLib;

    /// <summary>
    /// Controller of creating and accepting versions.
    /// </summary>
    public class VersionControl
    {
        /// <summary>
        /// Gets or sets the version list.
        /// </summary>
        /// <value>The version list.</value>
        public VersionList VersionList { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.VersionControl"/> class.
        /// </summary>
        public VersionControl()
        {
            VersionList = new VersionList();
        }

        /// <summary>
        /// File created.
        /// </summary>
        /// <param name="e">E.</param>
        public void Created(FileChangedEventArgs e)
        {
            // TODO comments
            Console.WriteLine("Created");
            VersionList.Create(e.Name, e.IsDirectory, e.When);
        }

        /// <summary>
        /// File changed.
        /// </summary>
        /// <param name="e">E.</param>
        public void Changed(FileChangedEventArgs e)
        {
            Console.WriteLine("Changed");
            VersionList.Change(e.Name, e.IsDirectory, e.SHA1, e.When);
        }

        /// <summary>
        /// File renamed.
        /// </summary>
        /// <param name="e">E.</param>
        public void Renamed(FileChangedEventArgs e)
        {
            Console.WriteLine("Renamed");
            VersionList.Rename(e.Name, e.OldName, e.SHA1, e.When);
        }

        /// <summary>
        /// File deleted.
        /// </summary>
        /// <param name="e">E.</param>
        public void Deleted(FileChangedEventArgs e)
        {
            Console.WriteLine("Deleted");
            VersionList.Delete(e.Name, e.When);
        }

        /// <summary>
        /// Flush version list to disk.
        /// </summary>
        public void Flush()
        {
            Console.WriteLine("Flush");
            VersionList.Flush();
        }

        /// <summary>
        /// Creates a bundle containing version list delta and all data of files.
        /// </summary>
        /// <returns>The path of bundle.</returns>
        /// <param name="list">List needed to transfered.</param>
        public string CreateFileBundle(List<FileEvent> list)
        {
            string tmpPathName = CommonHelper.GetRandomHash();
            string bundlePath = Config.MetaFolderTmp.File(tmpPathName + Properties.BundleFileExt);
            AbsolutePath tmpPath = Config.MetaFolderTmp.Enter(tmpPathName);
            Directory.CreateDirectory(tmpPath);
            
            list.WriteObject(tmpPath + Properties.PathSep + Properties.DeltaFile);
            foreach (var sha1 in list.Where(x => x.SHA1 != null).Select(x => x.SHA1).Distinct())
            {
                File.Copy(Config.MetaFolderData.File(sha1), tmpPath.File(sha1));
            }

            CommonHelper.Zip(bundlePath, tmpPath);
            Directory.Delete(tmpPath, true);
            return bundlePath;
        }

        /// <summary>
        /// Accepts a file bundle containing all data of files.
        /// </summary>
        /// <param name="data">Binary data.</param>
        public List<FileEvent> AcceptFileBundle(byte[] data)
        {
            string tmpPathName = CommonHelper.GetRandomHash();
            string bundlePath = Config.MetaFolderTmp.File(tmpPathName + Properties.BundleFileExt);
            AbsolutePath tmpPath = Config.MetaFolderTmp.Enter(tmpPathName);
            Directory.CreateDirectory(tmpPath);
            File.WriteAllBytes(bundlePath, data);
            CommonHelper.UnZip(bundlePath, tmpPath);

            // Copy all files
            foreach (var file in Directory.GetFiles(tmpPath))
            {
                FileInfo info = new FileInfo(file);
                if (info.Name == Properties.DeltaFile)
                {
                    continue;
                }

                if (File.Exists(Config.MetaFolderData.File(info.Name)))
                {
                    Console.WriteLine("File exists: {0}", Config.MetaFolderData.File(info.Name));
                    continue;
                }

                File.Copy(info.FullName, Config.MetaFolderData.File(info.Name));
            }

            // Append versions
            Dictionary<string, FileItem> myFileList = new Dictionary<string, FileItem>();
            foreach (var item in VersionList.AllFiles)
            {
                myFileList[item.Id] = item;
            }

            var myPatchList = CommonHelper.ReadObject<List<FileEvent>>(tmpPath.File(Properties.DeltaFile));
            foreach (var patch in myPatchList)
            {
                if (!myFileList.ContainsKey(patch.FileId))
                {
                    myFileList[patch.FileId] = new FileItem(patch.FileId);
                    VersionList.AllFiles.Add(myFileList[patch.FileId]);
                    VersionList.SetFileByName(patch.Name, myFileList[patch.FileId]);
                }

                string oldName = myFileList[patch.FileId].CurrentName;
                myFileList[patch.FileId].Merge(patch);
                string currentName = myFileList[patch.FileId].CurrentName;
                if (oldName != currentName)
                {
                    VersionList.RemoveFileByName(oldName);
                    VersionList.SetFileByName(patch.Name, myFileList[patch.FileId]);
                }
            }

            // Clean up
            // File.Delete(tmpPath + Properties.BundleFileExt);
            Directory.Delete(tmpPath, true);
            this.Flush();

            return myPatchList;
        }
    }
}
