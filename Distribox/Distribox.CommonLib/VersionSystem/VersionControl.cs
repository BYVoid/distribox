using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class VersionControl
    {
		/// <summary>
		/// The sync root of Distribox.
		/// </summary>
        private String _root;

        public VersionList VersionList { get; set; }

        public VersionControl(String root)
        {
			this._root = root;
			Initialize();
			VersionList = new VersionList(root + ".Distribox/VersionList.txt");
        }

		private void Initialize()
		{
			if (!Directory.Exists(_root))
			{
				Directory.CreateDirectory(_root);
			}
			if (!Directory.Exists(_root + ".Distribox"))
			{
				Directory.CreateDirectory(_root + ".Distribox");
			}
			if (!Directory.Exists(_root + ".Distribox/tmp"))
			{
				Directory.CreateDirectory(_root + ".Distribox/tmp");
			}
			if (!Directory.Exists(_root + ".Distribox/data"))
			{
				Directory.CreateDirectory(_root + ".Distribox/data");
			}
			if (!File.Exists(_root + ".Distribox/VersionList.txt"))
			{
				File.WriteAllText(_root + ".Distribox/VersionList.txt", "[]");
			}
		}

        public void Created(FileChangedEventArgs e)
        {
            Console.WriteLine("Created");
            VersionList.Create(e.Name, e.IsDirectory, e.When);
        }

        public void Changed(FileChangedEventArgs e)
        {
            Console.WriteLine("Changed");
            VersionList.Change(e.Name, e.IsDirectory, e.SHA1, e.When);
        }

        public void Renamed(FileChangedEventArgs e)
        {
            Console.WriteLine("Renamed");
            VersionList.Rename(e.Name, e.OldName, e.SHA1, e.When);
        }

        public void Deleted(FileChangedEventArgs e)
        {
            Console.WriteLine("Deleted");
            VersionList.Delete(e.Name, e.When);
        }

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
		public string CreateFileBundle(List<FileItem> list)
		{
			string dataPath = _root + ".Distribox/data/";
			string tmpPath = _root + ".Distribox/tmp/" + CommonHelper.GetRandomHash();
			Directory.CreateDirectory(tmpPath);
			list.WriteObject(tmpPath + "/Delta.txt");
			foreach (var item in list)
			{
				foreach (var history in item.History)
				{
					if (history.SHA1 == null)
						continue;
					File.Copy(dataPath + history.SHA1, tmpPath + "/" + history.SHA1);
				}
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
			var list = CommonHelper.ReadObject<List<FileItem>>(tmpPath + "/Delta.txt");
			foreach (var item in list)
			{
				if (!myFileList.ContainsKey(item.Id))
				{
					myFileList[item.Id] = new FileItem(item.Id);
					VersionList.AllFiles.Add(myFileList[item.Id]);
					VersionList.SetFileByName(myFileList[item.Id].CurrentName, myFileList[item.Id]);
				}
				foreach (var history in item.History)
				{
					myFileList[item.Id].NewVersion(history);
				}
			}
			
			// Clean up
			File.Delete(tmpPath + ".zip");
			Directory.Delete(tmpPath, true);
			Flush();
		}
    }
}
