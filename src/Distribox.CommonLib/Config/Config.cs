//-----------------------------------------------------------------------
// <copyright file="Config.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.CommonLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Configuration of all properties used in runtime.
    /// </summary>
    /// <remarks>
    /// This class should be implemented as static class, because
    /// using Config.GetConfig().Property is awkward. Use Config.Property
    /// is preferred. However, we cannot serialize a static class...
    /// </remarks>
    public class Config
    {
        /// <summary>
        /// The config data.
        /// </summary>
        private static ConfigData configData = null;

        /// <summary>
        /// Gets the root folder.
        /// </summary>
        /// <value>The root folder.</value>
        public static AbsolutePath RootFolder
        {
            get
            {
                EnsureInitialized();
                return new AbsolutePath(configData.RootFolder);
            }
        }

        /// <summary>
        /// Gets the meta folder.
        /// </summary>
        /// <value>The meta folder.</value>
        public static AbsolutePath MetaFolder
        {
            get
            {
                EnsureInitialized();
                return RootFolder.Enter(Properties.MetaFolder);
            }
        }

        /// <summary>
        /// Gets the meta folder data.
        /// </summary>
        /// <value>The meta folder data.</value>
        public static AbsolutePath MetaFolderData
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.Enter(Properties.MetaFolderData);
            }
        }

        /// <summary>
        /// Gets the meta folder temp.
        /// </summary>
        /// <value>The meta folder temp.</value>
        public static AbsolutePath MetaFolderTmp
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.Enter(Properties.MetaFolderTmp);
            }
        }

        /// <summary>
        /// Gets the version list file path.
        /// </summary>
        /// <value>The version list file path.</value>
        public static string VersionListFilePath
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.File(Properties.VersionListFile);
            }
        }

        public static string PeerBandwidthFilePath
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.File(Properties.PeerBandwidthFile);
            }
        }

        /// <summary>
        /// Gets the peer list file path.
        /// </summary>
        /// <value>The peer list file path.</value>
        public static string PeerListFilePath
        {
            get
            {
                EnsureInitialized();
                return MetaFolder.File(Properties.PeerListFile);
            }
        }

        /// <summary>
        /// Gets the listen port.
        /// </summary>
        /// <value>The listen port.</value>
        public static int ListenPort
        {
            get
            {
                EnsureInitialized();
                return configData.ListenPort;
            }
        }

        /// <summary>
        /// Gets the connect period millisecond.
        /// </summary>
        /// <value>The connect period millisecond.</value>
        public static int ConnectPeriodMs
        {
            get
            {
                EnsureInitialized();
                return configData.ConnectPeriodMs;
            }
        }

        /// <summary>
        /// Gets the file watcher time interval millisecond.
        /// </summary>
        /// <value>The file watcher time interval millisecond.</value>
        public static int FileWatcherTimeIntervalMs
        {
            get
            {
                EnsureInitialized();
                return configData.FileWatcherTimeIntervalMs;
            }
        }

        /// <summary>
        /// Ensures the initialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (configData != null)
            {
                return;
            }

            if (File.Exists(Properties.ConfigFileName))
            {
                // Read from existing config
                configData = CommonHelper.ReadObject<ConfigData>(Properties.ConfigFileName);
            }
            else
            {
                // Create a new one
                Logger.Warn("{0} not found in {1}. Creating a default one...", Properties.ConfigFileName, Directory.GetCurrentDirectory());

                configData = new ConfigData();
                configData.SetDefault();
            }
        }
    }
}
