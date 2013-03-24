using Distribox.CommonLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class VersionList
    {
        public List<FileItem> AllFiles { get; set; }

        private Dictionary<String, FileItem> PathToFile = new Dictionary<String, FileItem>();

        private String _root;

        public VersionList() { }

        public VersionList(String root)
        {
            this._root = root;

            Initialize();

            AllFiles = CommonHelper.ReadObject<List<FileItem>>(root + ".Distribox/VersionList.txt");
            foreach (var file in AllFiles.Where(x => x.IsAlive))
            {
                PathToFile[file.CurrentName] = file;
            }
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

        public Boolean ContainsName(String Name)
        {
            return PathToFile.ContainsKey(Name);
        }

        public FileItem Create(String Name, Boolean IsDirectory, DateTime When)
        {
            if (PathToFile.ContainsKey(Name)) return null;

            FileItem item = new FileItem(Name, IsDirectory);
            item.Created(When);

            AllFiles.Add(item);

            PathToFile[Name] = item;

            return item;
        }

        public void Changed(String Name, Boolean IsDirectory, String SHA1, DateTime When)
        {
            if (!PathToFile.ContainsKey(Name))
            {
                Create(Name, IsDirectory, When);
            }

            if (IsDirectory) return;

            FileItem item = PathToFile[Name];
            item.Changed(Name, SHA1, When);
        }

        public void Renamed(String Name, String OldName, String SHA1, DateTime When)
        {
            FileItem item = PathToFile[OldName];

            item.Renamed(Name, When);

            PathToFile.Remove(OldName);
            PathToFile[Name] = item;
        }

        public void Delete(String Name, DateTime When)
        {
            FileItem item = PathToFile[Name];
            item.Deleted(When);
            PathToFile.Remove(Name);
        }

        public String GetFileBySHA1(String SHA1)
        {
            return _root + ".Distribox/data/" + SHA1;
        }

        public String CreateFileBundle(List<FileItem> list)
        {
            String data_path = _root + ".Distribox/data/";
            String tmp_path = _root + ".Distribox/tmp/" + CommonHelper.GetRandomHash();
            Directory.CreateDirectory(tmp_path);
            CommonHelper.WriteObject(tmp_path + "/Delta.txt", list);
            foreach (var item in list)
                foreach (var history in item.History)
                {
                    if (history.SHA1 == null) continue;
                    File.Copy(data_path + history.SHA1, tmp_path + "/" + history.SHA1);
                }
            CommonHelper.Zip(tmp_path + ".zip", tmp_path);
            Directory.Delete(tmp_path, true);
            return tmp_path + ".zip";
        }
       
        public void AcceptFileBundle(byte[] data)
        {
            String data_path = _root + ".Distribox/data/";
            String tmp_path = _root + ".Distribox/tmp/" + CommonHelper.GetRandomHash();
            Directory.CreateDirectory(tmp_path);
            File.WriteAllBytes(tmp_path + ".zip", data);
            CommonHelper.UnZip(tmp_path + ".zip", tmp_path);

            // copy all files
            foreach (var file in Directory.GetFiles(tmp_path))
            {
                FileInfo info = new FileInfo(file);
                if (info.Name == "Delta.txt") continue;
                if (File.Exists(data_path + info.Name)) continue;
                File.Copy(tmp_path + "/" + info.Name, data_path + info.Name);
            }

            // append versions
            Dictionary<String, FileItem> myFileList = new Dictionary<String, FileItem>();
            foreach (var item in AllFiles)
                myFileList[item.Id] = item;

            var list = CommonHelper.ReadObject<List<FileItem>>(tmp_path + "/Delta.txt");
            foreach (var item in list)
            {
                if (!myFileList.ContainsKey(item.Id))
                {
                    myFileList[item.Id] = new FileItem(item.Id);
                    AllFiles.Add(myFileList[item.Id]);
                    PathToFile[myFileList[item.Id].CurrentName] = myFileList[item.Id];
                }
                foreach (var history in item.History)
                {
                    myFileList[item.Id].NewVersion(history);
                }
            }

            File.Delete(tmp_path + ".zip");
            Directory.Delete(tmp_path, true);
            Flush();
        }

        public List<FileItem> GetLessThan(VersionList list)
        {
            HashSet<String> myFileList = new HashSet<String>();
            foreach (var item in AllFiles)
                foreach (var history in item.History)
                {
                    myFileList.Add(item.Id + "@" + history.SHA1);
                }

            Dictionary<String, FileItem> dict = new Dictionary<String, FileItem>();
            foreach (var item in list.AllFiles)
                foreach (var history in item.History)
                {
                    String guid = item.Id + "@" + history.SHA1;
                    if (myFileList.Contains(guid)) continue;
                    if (!dict.ContainsKey(item.Id)) dict[item.Id] = new FileItem(item.Id);
                    dict[item.Id].NewVersion(history);
                }
            return dict.Values.ToList();
        }

        public void Flush()
        {
            CommonHelper.WriteObject(_root + ".Distribox/VersionList.txt", AllFiles);
        }
    }
}
