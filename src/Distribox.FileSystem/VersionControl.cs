//-----------------------------------------------------------------------
// <copyright file="VersionControl.cs" company="CompanyName">
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
    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    /// Controller of creating and accepting versions.
    /// </summary>
    public class VersionControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.FileSystem.VersionControl"/> class.
        /// </summary>
        public VersionControl()
        {
            VersionList = new VersionList();
        }

        /// <summary>
        /// Gets or sets the version list.
        /// </summary>
        /// <value>The version list.</value>
        public VersionList VersionList { get; set; }

        /// <summary>
        /// File created.
        /// </summary>
        /// <param name="e">The event.</param>
        public void Created(FileChangedEventArgs e)
        {
            // TODO comments
            Console.WriteLine("Created");
            VersionList.Create(e.Name, e.IsDirectory, e.When);
        }

        /// <summary>
        /// File changed.
        /// </summary>
        /// <param name="e">The event.</param>
        public void Changed(FileChangedEventArgs e)
        {
            Console.WriteLine("Changed");
            VersionList.Change(e.Name, e.IsDirectory, e.SHA1, e.When);
        }

        /// <summary>
        /// File renamed.
        /// </summary>
        /// <param name="e">The event.</param>
        public void Renamed(FileChangedEventArgs e)
        {
            Console.WriteLine("Renamed");
            VersionList.Rename(e.Name, e.OldName, e.SHA1, e.When);
        }

        /// <summary>
        /// File deleted.
        /// </summary>
        /// <param name="e">The event.</param>
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

        public void CheckOut(string fileId, string eventId)
        {
            FileItem file = VersionList.GetFileById(fileId);

            file.CheckOut(eventId);

            VersionList.Flush();

        }

        /// <summary>
        /// Creates a bundle containing version list delta and all data of files.
        /// </summary>
        /// <returns>The binary bundle.</returns>
        /// <param name="list">List needed to transferred.</param>
        public static byte[] CreateFileBundle(List<FileEvent> list)
        {
            using (MemoryStream ms = new MemoryStream())
            using (ZipOutputStream zip = new ZipOutputStream(ms))
            {
                ZipEntry block = new ZipEntry("vs");
                zip.PutNextEntry(block);
                zip.WriteAllBytes(list.SerializeAsBytes());
                zip.CloseEntry();

                foreach (var sha1 in list.Where(x => x.SHA1 != null).Select(x => x.SHA1).Distinct())
                {
                    block = new ZipEntry(sha1);
                    zip.PutNextEntry(block);
                    zip.WriteAllBytes(File.ReadAllBytes(Config.MetaFolderData.File(sha1)));
                    zip.CloseEntry();
                }

                zip.Finish();
                ms.Flush();
                ms.Position = 0;

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Accepts a file bundle containing all data of files.
        /// </summary>
        /// <param name="data">Binary data.</param>
        /// <returns>List of events.</returns>
        public List<FileEvent> AcceptFileBundle(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (ZipInputStream zip = new ZipInputStream(ms))
            {
                ZipEntry block = zip.GetNextEntry();
                var myPatchList = zip.ReadAllBytes().Deserialize<List<FileEvent>>();

                while (true)
                {
                    block = zip.GetNextEntry();
                    if (block == null) break;
                    File.WriteAllBytes(Config.MetaFolderData.File(block.Name), zip.ReadAllBytes());
                }

                // Append versions
                Dictionary<string, FileItem> myFileList = new Dictionary<string, FileItem>();
                foreach (var item in VersionList.AllFiles)
                {
                    myFileList[item.Id] = item;
                }

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

                return myPatchList;
            }
        }
    }
}
