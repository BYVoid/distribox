using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Distribox.FileSystem
{
    public class FileChangedEventArgs
    {
        public WatcherChangeTypes ChangeType { get; set; }
        public String FullPath { get; set; }
        public String Name { get; set; }
        public String OldFullPath { get; set; }
        public String OldName { get; set; }
        public String SHA1 { get; set; }
        public String DataPath { get; set; }
        public DateTime When { get; set; }
        public Boolean IsDirectory { get; set; }
    }
    
}
