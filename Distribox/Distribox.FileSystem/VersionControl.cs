using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Distribox.CommonLib;

namespace Distribox.FileSystem
{
    public class VersionControl
    {
        /// <summary>
        /// The sync root of Distribox.
        /// </summary>
        private string _root;

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
            this._root = root;
            VersionList = new VersionList(root + ".Distribox/VersionList.txt");
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
            string dataPath = _root + ".Distribox/data/";
            string tmpPath = _root + ".Distribox/tmp/" + CommonHelper.GetRandomHash();
            Directory.CreateDirectory(tmpPath);
            list.WriteObject(tmpPath + "/Delta.txt");
            foreach (var patch in list.Where(x => x.SHA1 != null))
            {
                File.Copy(dataPath + patch.SHA1, tmpPath + "/" + patch.SHA1);
            }
            CommonHelper.Zip(tmpPath + ".zip", tmpPath);
            Directory.Delete(tmpPath, true);
            return tmpPath + ".zip";
        }

        /// <summary>
        /// Accepts a file bundle containing version list delta and all data of files.
        /// </summary>
        /// <param name="data">Binary data.</param>
        public void AcceptFileBundle(byte[] data)
        {
            string dataPath = _root + ".Distribox/data/";
            string tmpPath = _root + ".Distribox/tmp/" + CommonHelper.GetRandomHash();
            Directory.CreateDirectory(tmpPath);
            File.WriteAllBytes(tmpPath + ".zip", data);
            CommonHelper.UnZip(tmpPath + ".zip", tmpPath);

            // Copy all files
            foreach (var file in Directory.GetFiles(tmpPath))
            {
                FileInfo info = new FileInfo(file);
                if (info.Name == "Delta.txt")
                    continue;
                if (File.Exists(dataPath + info.Name))
                    continue;
                File.Copy(tmpPath + "/" + info.Name, dataPath + info.Name);
            }

            // Append versions
            Dictionary<string, FileItem> myFileList = new Dictionary<string, FileItem>();
            foreach (var item in VersionList.AllFiles)
            {
                myFileList[item.Id] = item;
            }
            var myPatchList = CommonHelper.ReadObject<List<AtomicPatch>>(tmpPath + "/Delta.txt");
            foreach (var patch in myPatchList)
            {
                if (!myFileList.ContainsKey(patch.Id))
                {
                    myFileList[patch.Id] = new FileItem(patch.Id);
                    VersionList.AllFiles.Add(myFileList[patch.Id]);
                    VersionList.SetFileByName(patch.Name, myFileList[patch.Id]);
                }
                myFileList[patch.Id].ApplyPatch(_root, patch);
            }

            // Clean up
            File.Delete(tmpPath + ".zip");
            Directory.Delete(tmpPath, true);
            Flush();
        }
    }
}
