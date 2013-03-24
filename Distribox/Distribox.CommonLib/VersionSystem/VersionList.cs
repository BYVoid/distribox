using Distribox.CommonLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
	// TODO this class would be serialized for deserialized
	/// <summary>
	/// Version list.
	/// </summary>
    public class VersionList
    {
		/// <summary>
		/// Gets or sets all files that ever existed.
		/// </summary>
		/// <value>All files.</value>
        public List<FileItem> AllFiles { get; set; }

		/// <summary>
		/// Maps relative path to file item object
		/// </summary>
        private Dictionary<string, FileItem> _pathToFile = new Dictionary<string, FileItem>();

		/// <summary>
		/// The sync root of Distribox.
		/// </summary>
        private string _root;

		/// <summary>
		/// Initializes a new instance of the <see cref="Distribox.CommonLib.VersionList"/> class.
		/// Only for serialization
		/// </summary>
        public VersionList() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Distribox.CommonLib.VersionList"/> class.
		/// </summary>
		/// <param name="root">Root.</param>
        public VersionList(string root)
        {
            this._root = root;

            Initialize();

			// Deserialize version list
            AllFiles = CommonHelper.ReadObject<List<FileItem>>(_root + ".Distribox/VersionList.txt");
            foreach (var file in AllFiles.Where(x => x.IsAlive))
            {
                _pathToFile[file.CurrentName] = file;
            }
        }

		// TODO move this to other place
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

		/// <summary>
		/// Create a file item
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="isDirectory">If set to <c>true</c> is directory.</param>
		/// <param name="when">When.</param>
        public FileItem Create(string name, bool isDirectory, DateTime when)
        {
            if (_pathToFile.ContainsKey(name))
				return null;

            FileItem item = new FileItem(name, isDirectory);
            item.Create(when);

            AllFiles.Add(item);

            _pathToFile[name] = item;

            return item;
        }

		/// <summary>
		/// Change a file item.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="isDirectory">If set to <c>true</c> is directory.</param>
		/// <param name="SHA1">SH a1.</param>
		/// <param name="when">When.</param>
		public void Change(string name, bool isDirectory, string SHA1, DateTime when)
        {
            if (!_pathToFile.ContainsKey(name))
            {
                Create(name, isDirectory, when);
            }

            if (isDirectory) return;

            FileItem item = _pathToFile[name];
            item.Change(name, SHA1, when);
        }

		/// <summary>
		/// Rename a file item.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="oldName">Old name.</param>
		/// <param name="SHA1">SH a1.</param>
		/// <param name="when">When.</param>
        public void Rename(string name, string oldName, string SHA1, DateTime when)
        {
            FileItem item = _pathToFile[oldName];

            item.Rename(name, when);

            _pathToFile.Remove(oldName);
            _pathToFile[name] = item;
        }

		/// <summary>
		/// Delete a file item.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="when">When.</param>
        public void Delete(string name, DateTime when)
        {
            FileItem item = _pathToFile[name];
            item.Delete(when);
            _pathToFile.Remove(name);
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
            foreach (var item in AllFiles)
			{
				myFileList[item.Id] = item;
			}
            var list = CommonHelper.ReadObject<List<FileItem>>(tmpPath + "/Delta.txt");
            foreach (var item in list)
            {
                if (!myFileList.ContainsKey(item.Id))
                {
                    myFileList[item.Id] = new FileItem(item.Id);
                    AllFiles.Add(myFileList[item.Id]);
                    _pathToFile[myFileList[item.Id].CurrentName] = myFileList[item.Id];
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

		// TODO use version
		/// <summary>
		/// Gets all version that exist in <paramref name="list"/> but not exist in this
		/// </summary>
		/// <returns>The less than.</returns>
		/// <param name="list">List.</param>
        public List<FileItem> GetLessThan(VersionList list)
        {
            HashSet<string> myFileList = new HashSet<string>();
            foreach (var item in AllFiles)
			{
				foreach (var history in item.History)
				{
					myFileList.Add(item.Id + "@" + history.SHA1);
				}
			}

            Dictionary<string, FileItem> dict = new Dictionary<string, FileItem>();
            foreach (var item in list.AllFiles)
			{
				foreach (var history in item.History)
				{
					string guid = item.Id + "@" + history.SHA1;
					if (myFileList.Contains(guid))
						continue;
					if (!dict.ContainsKey(item.Id))
						dict[item.Id] = new FileItem(item.Id);
					dict[item.Id].NewVersion(history);
				}
			}
            return dict.Values.ToList();
        }

		/// <summary>
		/// Flush version list into disk
		/// </summary>
        public void Flush()
        {
			AllFiles.WriteObject(_root + ".Distribox/VersionList.txt");
        }
    }
}
