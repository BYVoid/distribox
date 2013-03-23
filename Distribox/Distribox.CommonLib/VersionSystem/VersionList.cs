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

        private String root;

        public VersionList() { }

        public VersionList(String root)
        {
            this.root = root;

            AllFiles = CommonHelper.ReadObject<List<FileItem>>(root + ".Distribox/VersionList.txt");
			// TODO empty version list
            foreach (var file in AllFiles.Where(x => x.IsAlive))
            {
                PathToFile[file.CurrentName] = file;
            }
			// TODO make folders .Distribox data tmp
        }

        public FileItem this[String Name]
        {
            get
            {
                return PathToFile[Name];
            }

            set
            {
                PathToFile[Name] = value;
            }
        }

        public Boolean ContainsName(String Name)
        {
            return PathToFile.ContainsKey(Name);
        }

        public FileItem Create(String Name, Boolean IsDirectory, DateTime When)
        {
            if (PathToFile.ContainsKey(Name)) return null;

            FileItem item = new FileItem(Name, IsDirectory, When);
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
            if (item.CurrentSHA1 == SHA1) return;

            item.NewVersion(Name, SHA1, When);
        }

        public void Renamed(String Name, String OldName, String SHA1, DateTime When)
        {
            FileItem item = PathToFile[OldName];

            item.NewVersion(Name, SHA1, When);

            PathToFile.Remove(OldName);
            PathToFile[Name] = item;
        }

        public void Delete(String Name, DateTime When)
        {
            FileItem item = PathToFile[Name];
            item.DeadTime = When;
            item.IsAlive = false;
            PathToFile.Remove(Name);
        }

        public String GetFileBySHA1(String SHA1)
        {
            return root + ".Distribox/data/" + SHA1;
        }

        public void Flush()
        {
            CommonHelper.WriteObject(root + ".Distribox/VersionList.txt", AllFiles);
        }

        public String CreateFileBundle(List<FileItem> list)
        {
            String data_path = root + ".Distribox/data/";
            String tmp_path = root + ".Distribox/tmp/" + CommonHelper.GetRandomHash();
            Directory.CreateDirectory(tmp_path);
            CommonHelper.WriteObject(tmp_path + "/Delta.txt", list);
            foreach (var item in list)
                foreach (var history in item.History)
                {
                    File.Copy(data_path + history.SHA1, tmp_path + "/" + history.SHA1);
                }
            CommonHelper.Zip(tmp_path, tmp_path + ".7z");
            Directory.Delete(tmp_path, true);
            return tmp_path + ".7z";
        }
       
        public void AcceptFileBundle(byte[] data)
        {
            String data_path = root + ".Distribox/data/";
            String tmp_path = root + ".Distribox/tmp/" + CommonHelper.GetRandomHash();
            Directory.CreateDirectory(tmp_path);
            File.WriteAllBytes(tmp_path + ".7z", data);
            CommonHelper.UnZip(tmp_path + ".7z", tmp_path);

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
                    myFileList[item.Id] = FileItem.CreateEmpty(item);
                    AllFiles.Add(myFileList[item.Id]);
                    PathToFile[myFileList[item.Id].CurrentName] = myFileList[item.Id];
                }
                foreach (var history in item.History)
                {
                    myFileList[item.Id].NewVersion(history.Name, history.SHA1, history.LastModify);
                }
            }

            File.Delete(tmp_path + ".7z");
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
                    if (!dict.ContainsKey(item.Id)) dict[item.Id] = FileItem.CreateEmpty(item);
                    dict[item.Id].NewVersion(history.Name, history.SHA1, history.LastModify);
                }
            return dict.Values.ToList();
        }
    }
}
