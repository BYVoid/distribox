using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Distribox.CommonLib
{
    /// <summary>
    /// Configuration of all properies used in runtime.
    /// </summary>
    /// <remarks>
    /// This class should be implemented as static class, because
    /// using Config.GetConfig().Property is awkward. Use Config.Property
    /// is preferred. However, we cannot serialize a static class...
    /// </remarks>
    public class Config
    {
        private static ConfigData configData = null;

        public static AbsolutePath RootFolder
        {
            get
            {
                EnsureInitialized();
                return new AbsolutePath(configData.RootFolder);
            }
        }

        public static AbsolutePath MetaFolder
        {
            get
            {
                EnsureInitialized();
                return RootFolder.Enter(Properties.MetaFolder);
            }
        }

        public static AbsolutePath MetaFolderData
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.Enter(Properties.MetaFolderData);
            }
        }

        public static AbsolutePath MetaFolderTmp
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.Enter(Properties.MetaFolderTmp);
            }
        }

        public static string VersionListFilePath
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.File(Properties.VersionListFile);
            }
        }

        public static string PeerListFilePath
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.File(Properties.PeerListFile);
            }
        }

        public static int ListenPort
        {
            get
            {
                EnsureInitialized();
                return configData.ListenPort;
            }
        }

        public static int ConnectPeriodMs
        {
            get
            {
                EnsureInitialized();
                return configData.ConnectPeriodMs;
            }
        }

        public static int FileWatcherTimeIntervalMs
        {
            get
            {
                EnsureInitialized();
                return configData.FileWatcherTimeIntervalMs;
            }
        }

        private static void EnsureInitialized()
        {
            if (configData != null) return;

            if (File.Exists(Properties.ConfigFileName))
            {
                // Read from existing config
                configData = CommonHelper.ReadObject<ConfigData>(Properties.ConfigFileName);
            }
            else
            {
                // Create a new one
                Logger.Warn("{0} not found in {1}. Creating a default one...",
                    Properties.ConfigFileName, Directory.GetCurrentDirectory());

                configData = new ConfigData();
                configData.SetDefault();
            }
        }
    }
}
