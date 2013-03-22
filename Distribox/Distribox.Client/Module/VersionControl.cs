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

        public VersionList version_list { get; set; }

        public VersionControl(String root)
        {
            this.root = root;
            version_list = new VersionList(root);
        }

        public void Created(FileChangedEventArgs e)
        {
            Console.WriteLine("Created");
            version_list.Create(e.Name, e.IsDirectory, e.When);
        }

        public void Changed(FileChangedEventArgs e)
        {
            Console.WriteLine("Changed");
            version_list.Changed(e.Name, e.IsDirectory, e.SHA1, e.When);
        }

        public void Renamed(FileChangedEventArgs e)
        {
            Console.WriteLine("Renamed");
            version_list.Renamed(e.Name, e.OldName, e.SHA1, e.When);
        }

        public void Deleted(FileChangedEventArgs e)
        {
            Console.WriteLine("Deleted");
            version_list.Delete(e.Name, e.When);
        }

        public void Flush()
        {
            Console.WriteLine("Flush");
            version_list.Flush();
        }
    }
}
