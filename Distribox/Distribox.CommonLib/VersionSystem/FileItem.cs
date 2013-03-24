using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class FileItem
    {
        public Boolean IsAlive { get; set; }
        public String Id { get; set; }
        public Boolean IsDirectory { get; set; }
        public DateTime DeadTime { get; set; }
        public String CurrentName { get; set; }
        public String CurrentSHA1 { get; set; }
        public List<FileSubversion> History { get; set; }

        public FileItem()
        {
            this.History = new List<FileSubversion>();
        }

        public FileItem(String Id)
        {
            this.History = new List<FileSubversion>();
            this.Id = Id;
        }

        public FileItem(String Name, Boolean IsDirectory)
        {
            this.History = new List<FileSubversion>();
            this.IsAlive = true;
            this.Id = CommonHelper.GetRandomHash();
            this.IsDirectory = IsDirectory;
            this.CurrentName = Name;
            this.CurrentSHA1 = null;
        }

        public void Created(DateTime When)
        {
            FileSubversion vs = new FileSubversion();
            vs.Type = FileSubversionType.Created;
            vs.Name = CurrentName;
            vs.LastModify = When;
            vs.SHA1 = null;

            CurrentSHA1 = null;
            History.Add(vs);
        }

        public void Renamed(String Name, DateTime When)
        {
            if (Name == CurrentName) return;

            FileSubversion vs = new FileSubversion();
            vs.Type = FileSubversionType.Renamed;
            vs.Name = Name;
            vs.LastModify = When;
            vs.SHA1 = CurrentSHA1;

            CurrentName = Name;
            History.Add(vs);
        }

        public void Deleted(DateTime When)
        {
            FileSubversion vs = new FileSubversion();
            vs.Type = FileSubversionType.Deleted;
            vs.Name = CurrentName;
            vs.LastModify = When;
            vs.SHA1 = CurrentSHA1;

            IsAlive = false;
            DeadTime = When;
            History.Add(vs);
        }

        public void Changed(String Name, String SHA1, DateTime When)
        {
            if (SHA1 == CurrentSHA1) return;

            FileSubversion vs = new FileSubversion();
            vs.Type = FileSubversionType.Changed;
            vs.Name = Name;
            vs.LastModify = When;
            vs.SHA1 = SHA1;

            CurrentSHA1 = SHA1;
            History.Add(vs);
        }

        public void NewVersion(FileSubversion vs)
        {
            History.Add(vs);
        }
    }
}
