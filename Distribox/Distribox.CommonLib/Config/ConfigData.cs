using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class ConfigData
    {
        public int ListenPort { get; set; }
        public int ConnectPeriodMs { get; set; }
        public int FileWatcherTimeIntervalMs { get; set; }
        public string RootFolder { get; set; }

        public void SetDefault()
        {
            ListenPort = Properties.DefaultListenPort;

            // Use working directory/shared as root folder
            RootFolder = Directory.GetCurrentDirectory();

            ConnectPeriodMs = Properties.DefaultConnectPeriodMs;

            FileWatcherTimeIntervalMs = Properties.DefaultFileWatcherTimeIntervalMs;

            CommonHelper.WriteObject(this, Properties.ConfigFileName);
        }
    }
}
