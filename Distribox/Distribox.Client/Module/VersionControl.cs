using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Distribox.CommonLib;

namespace Distribox.Client.Module
{
    public class FileSubversion
    {
        public String Name;
        public String SHA1;
        public DateTime LastModify;
    }

    public class FileItem
    {
        public Boolean IsAlive;
        public String Id;
        public Boolean IsDirectory;
        public DateTime DeadTime;
        public String CurrentName;

        public List<FileSubversion> History = new List<FileSubversion>();
    }

    public class VersionControl
    {
        private List<FileItem> AllFiles = new List<FileItem>();
        private Dictionary<String, FileItem> PathToFile = new Dictionary<String, FileItem>();

        private String root;

        public VersionControl(String root)
        {
            this.root = root;
            Initalize();
        }

        private void Initalize()
        {
            AllFiles = CommonHelper.ReadObject<List<FileItem>>(root + ".Distribox\\VersionList.txt");
            foreach (var file in AllFiles.Where(x => x.IsAlive))
            {
                PathToFile[file.CurrentName] = file;
            }
        }

        public void Created(FileChangedEventArgs e)
        {
            if (PathToFile.ContainsKey(e.Name)) return;

            FileItem item = new FileItem();
            item.IsAlive = true;
            item.Id = CommonHelper.GetRandomHash();
            item.IsDirectory = e.IsDirectory;
            item.CurrentName = e.Name;

            if (e.IsDirectory)
            {
                FileSubversion vs = new FileSubversion();
                vs.Name = e.Name;
                vs.LastModify = e.LastModify;
                vs.SHA1 = null;

                item.CurrentName = e.Name;
                PathToFile[vs.Name] = item;
                item.History.Add(vs);
            }

            AllFiles.Add(item);
            PathToFile[e.Name] = item;
        }

        public void Changed(FileChangedEventArgs e)
        {
            if (!PathToFile.ContainsKey(e.Name))
            {
                FileItem item = new FileItem();
                item.IsAlive = true;
                item.Id = CommonHelper.GetRandomHash();
                item.IsDirectory = e.IsDirectory;
                item.CurrentName = e.Name;

                AllFiles.Add(item);
                PathToFile[e.Name] = item;
            }

            if (e.IsDirectory)
            {
                return;
            }
            else
            {
                FileItem item = PathToFile[e.Name];
                if (item.History.Count() != 0 && item.History.Last().SHA1 == e.SHA1) return;

                FileSubversion vs = new FileSubversion();
                vs.Name = e.Name;
                vs.LastModify = e.LastModify;
                vs.SHA1 = e.SHA1;

                item.CurrentName = e.Name;
                PathToFile[vs.Name] = item;
                item.History.Add(vs);
            }
        }

        public void Renamed(FileChangedEventArgs e)
        {
            FileItem item = PathToFile[e.OldName];

            FileSubversion vs = new FileSubversion();
            vs.Name = e.Name;
            vs.LastModify = e.LastModify;
            vs.SHA1 = e.SHA1;

            item.CurrentName = e.Name;
            PathToFile[vs.Name] = item;
            item.History.Add(vs);
        }

        public void Deleted(FileChangedEventArgs e)
        {
            FileItem item = PathToFile[e.Name];
            item.DeadTime = e.LastModify;
            PathToFile.Remove(e.Name);
            item.IsAlive = false;
        }

        public void Flush()
        {
            CommonHelper.WriteObject(root + ".Distribox\\VersionList.txt", AllFiles);
        }
    }
}
