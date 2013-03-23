using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Distribox.CommonLib;

namespace Distribox.Client.Module
{
    public class VersionControl
    {
        private String root;

        public VersionList VersionList { get; set; }

        public VersionControl(String root)
        {
            this.root = root;
            VersionList = new VersionList(root);
        }

        public void Created(FileChangedEventArgs e)
        {
            Console.WriteLine("Created");
            VersionList.Create(e.Name, e.IsDirectory, e.When);
        }

        public void Changed(FileChangedEventArgs e)
        {
            Console.WriteLine("Changed");
            VersionList.Changed(e.Name, e.IsDirectory, e.SHA1, e.When);
        }

        public void Renamed(FileChangedEventArgs e)
        {
            Console.WriteLine("Renamed");
            VersionList.Renamed(e.Name, e.OldName, e.SHA1, e.When);
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
    }
}
