using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class FileItem
    {
        public Boolean IsAlive;
        public String Id;
        public Boolean IsDirectory;
        public DateTime DeadTime;
        public String CurrentName;
        public String CurrentSHA1;
        public List<FileSubversion> History = new List<FileSubversion>();

        public FileItem(String Name, Boolean IsDirectory, DateTime When)
        {
            this.IsAlive = true;
            this.Id = CommonHelper.GetRandomHash();
            this.IsDirectory = IsDirectory;
            this.CurrentName = Name;
            this.CurrentSHA1 = null;
            if (IsDirectory) NewVersion(Name, null, When);
        }

        public void NewVersion(String Name, String SHA1, DateTime When)
        {
            if (SHA1 == CurrentSHA1 && Name == CurrentName) return;

            FileSubversion vs = new FileSubversion();
            vs.Name = Name;
            vs.LastModify = When;
            vs.SHA1 = SHA1;

            CurrentName = Name;
            CurrentSHA1 = SHA1;
            History.Add(vs);
        }
    }
}
