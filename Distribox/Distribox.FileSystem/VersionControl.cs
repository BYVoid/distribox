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
        /// The sync root of Distribox.
        /// </summary>
        private string root;

        /// <summary>
        /// Gets or sets the version list.
        /// </summary>
        /// <value>The version list.</value>
        public VersionList VersionList { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.VersionControl"/> class.
        /// </summary>
        /// <param name="root">Root.</param>
        public VersionControl(string root)
        {
            this.root = root;
            VersionList = new VersionList(root + Properties.VersionListFilePath);
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
        public string CreateFileBundle(List<AtomicPatch> list)
        {
            string dataPath = this.root + Properties.MetaFolderData;
            string tmpPath = this.root + Properties.MetaFolderTmp + Properties.PathSep + CommonHelper.GetRandomHash();
            Directory.CreateDirectory(tmpPath);
            list.WriteObject(tmpPath + Properties.PathSep + Properties.DeltaFile);
            foreach (var patch in list.Where(x => x.SHA1 != null))
            {
                File.Copy(dataPath + Properties.PathSep + patch.SHA1, tmpPath + Properties.PathSep + patch.SHA1);
            }

            CommonHelper.Zip(tmpPath + Properties.BundleFileExt, tmpPath);
            Directory.Delete(tmpPath, true);
            return tmpPath + Properties.BundleFileExt;
        }

        /// <summary>
        /// Accepts a file bundle containing all data of files.
        /// </summary>
        /// <param name="data">Binary data.</param>
        public List<AtomicPatch> AcceptFileBundle(byte[] data)
        {
            string tmpPath = this.root + Properties.MetaFolderTmp + Properties.PathSep + CommonHelper.GetRandomHash();
            Directory.CreateDirectory(tmpPath);
            File.WriteAllBytes(tmpPath + Properties.BundleFileExt, data);
            CommonHelper.UnZip(tmpPath + Properties.BundleFileExt, tmpPath);

            // Copy all files
            foreach (var file in Directory.GetFiles(tmpPath))
            {
                FileInfo info = new FileInfo(file);
                if (info.Name == Properties.DeltaFile)
                {
                    continue;
                }

                if (File.Exists(Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + info.Name))
                {
                    Console.WriteLine("File exists: {0}", Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + info.Name);
                    continue;
                }

                File.Copy(tmpPath + Properties.PathSep + info.Name, Config.GetConfig().RootFolder + Properties.MetaFolderData + Properties.PathSep + info.Name);
            }

            // Append versions
            Dictionary<string, FileItem> myFileList = new Dictionary<string, FileItem>();
            foreach (var item in VersionList.AllFiles)
            {
                myFileList[item.Id] = item;
            }

            var myPatchList = CommonHelper.ReadObject<List<AtomicPatch>>(tmpPath + Properties.PathSep + Properties.DeltaFile);
            foreach (var patch in myPatchList)
            {
                if (!myFileList.ContainsKey(patch.Id))
                {
                    myFileList[patch.Id] = new FileItem(patch.Id);
                    VersionList.AllFiles.Add(myFileList[patch.Id]);
                    VersionList.SetFileByName(patch.Name, myFileList[patch.Id]);
                }

                myFileList[patch.Id].ApplyPatch(this.root, patch);
            }

            // Clean up
            // File.Delete(tmpPath + Properties.BundleFileExt);
            Console.WriteLine("File bundle: {0}", tmpPath + Properties.BundleFileExt);
            Directory.Delete(tmpPath, true);
            this.Flush();

            Console.WriteLine("Count: {0}", myFileList.Count());
            return myPatchList;
        }
    }
}
