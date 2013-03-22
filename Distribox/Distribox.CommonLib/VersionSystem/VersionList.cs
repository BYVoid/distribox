using Distribox.CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class VersionList
    {
        private List<FileItem> _AllFiles = new List<FileItem>();
        public List<FileItem> AllFiles
        {
            get
            {
                return _AllFiles;
            }
        }
        private Dictionary<String, FileItem> PathToFile = new Dictionary<String, FileItem>();

        private String root;

        public VersionList(String root)
        {
            this.root = root;

            _AllFiles = CommonHelper.ReadObject<List<FileItem>>(root + ".Distribox\\VersionList.txt");
            foreach (var file in _AllFiles.Where(x => x.IsAlive))
            {
                PathToFile[file.CurrentName] = file;
            }
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
            _AllFiles.Add(item);

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
            return root + ".Distribox\\data\\" + SHA1;
        }

        public void Flush()
        {
            CommonHelper.WriteObject(root + ".Distribox\\VersionList.txt", _AllFiles);
        }

        public byte[] CreateFileBundle(List<FileItem> list)
        {
            throw new NotImplementedException();
        }

        public void AcceptFileBundle(byte[] data)
        {
            throw new NotImplementedException();
        }

        public List<FileItem> GetLessThan(VersionList list)
        {
            throw new NotImplementedException();
        }
    }
}
