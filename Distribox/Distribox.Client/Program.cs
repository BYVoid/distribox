using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.Client.Module;
using System.IO;

namespace DBox.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            String root = @"D:\Distribox\";
            VersionControl vs = new VersionControl(root);

            FileWatcher watcher = new FileWatcher(root);
            watcher.Created += vs.Created;
            watcher.Changed += vs.Changed;
            watcher.Deleted += vs.Deleted;
            watcher.Renamed += vs.Renamed;
            watcher.Idle += vs.Flush;
            watcher.WaitForEvent();
        }
    }
}
