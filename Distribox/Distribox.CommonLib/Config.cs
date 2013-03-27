using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Distribox.CommonLib
{    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This class should be implemented as static class, because
    /// using Config.GetConfig().Property is awkward. Use Config.Property
    /// is preferred. However, we cannot serialize a static class...
    /// </remarks>
    public class Config
    {
        public int ListenPort { get; set; }
        public string RootFolder { get; set; }
        public int ConnectPeriodMs { get; set; }
        public int FileWatcherTimeIntervalMs { get; set; }

        private static Config _config = null;
        private Config() { }

        public static Config GetConfig()
        {
            if (_config == null)
            {
                if (File.Exists(Properties.ConfigFileName))
                {
                    // Read from existing config
                    _config = CommonHelper.ReadObject<Config>(Properties.ConfigFileName);
                }
                else
                {
                    // Create a new one
                    Logger.Warn("{0} not found in {1}. Creating a default one...",
                        Properties.ConfigFileName, Directory.GetCurrentDirectory());

                    _config = new Config();
                    _config.CreateDefaultConfig();
                }
            }

            return _config;
        }

        public void Flush()
        {
            CommonHelper.WriteObject(this, Properties.ConfigFileName);
        }

        public void CreateDefaultConfig()
        {
            ListenPort = Properties.DefaultListenPort;

            // Use working directory/shared as root folder
            RootFolder = Directory.GetCurrentDirectory() + Properties.PathSep + Properties.DefaultRootFolder;

            ConnectPeriodMs = Properties.DefaultConnectPeriodMs;

            FileWatcherTimeIntervalMs = Properties.DefaultFileWatcherTimeIntervalMs;

            Flush();
        }
    }
}
