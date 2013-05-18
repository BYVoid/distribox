//-----------------------------------------------------------------------
// <copyright file="Properties.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.CommonLib
{
    /// <summary>
    /// The properties.
    /// </summary>
    public static class Properties
    {
        /// <summary>
        /// Gets the path sep.
        /// </summary>
        /// <value>The path sep.</value>
        public static string PathSep
        {
            get
            {
                return "/";
            }
        }

        /// <summary>
        /// Gets the meta folder.
        /// </summary>
        /// <value>The meta folder.</value>
        public static string MetaFolder
        {
            get
            {
                return ".Distribox";
            }
        }

        /// <summary>
        /// Gets the meta folder temp.
        /// </summary>
        /// <value>The meta folder temp.</value>
        public static string MetaFolderTmp
        {
            get
            {
                return "tmp";
            }
        }

        /// <summary>
        /// Gets the meta folder data.
        /// </summary>
        /// <value>The meta folder data.</value>
        public static string MetaFolderData
        {
            get
            {
                return "data";
            }
        }

        /// <summary>
        /// Gets the bundle file ext.
        /// </summary>
        /// <value>The bundle file ext.</value>
        public static string BundleFileExt
        {
            get
            {
                return ".zip";
            }
        }

        /// <summary>
        /// Gets the delta file.
        /// </summary>
        /// <value>The delta file.</value>
        public static string DeltaFile
        {
            get
            {
                return "Delta.txt";
            }
        }

        /// <summary>
        /// Gets the version list file.
        /// </summary>
        /// <value>The version list file.</value>
        public static string VersionListFile
        {
            get
            {
                return "VersionList.txt";
            }
        }

        /// <summary>
        /// Gets the peer list file.
        /// </summary>
        /// <value>The peer list file.</value>
        public static string PeerListFile
        {
            get
            {
                return "PeerList.json";
            }
        }

        /// <summary>
        /// Gets the peer bandwidth.
        /// </summary>
        public static string PeerBandwidthFile
        {
            get
            {
                return "PeerBandwidth.json";
            }
        }

        /// <summary>
        /// Gets the name of the config file.
        /// </summary>
        /// <value>The name of the config file.</value>
        public static string ConfigFileName
        {
            get
            {
                return "config.json";
            }
        }

        /// <summary>
        /// Gets the default listen port.
        /// </summary>
        /// <value>The default listen port.</value>
        public static int DefaultListenPort
        {
            get
            {
                return 7777;
            }
        }

        /// <summary>
        /// Gets the default bandwidth.
        /// </summary>
        public static int DefaultBandwidth
        {
            get
            {
                return 10 * 1024 * 1024;
            }
        }

        /// <summary>
        /// Gets the default delay.
        /// </summary>
        public static int DefaultDelay
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// Gets the default connection speed with a peer.
        /// </summary>
        public static int DefaultConnectionSpeed
        {
            get
            {
                return 20 * 1024;
            }
        }

        /// <summary>
        /// Gets the default root folder.
        /// </summary>
        /// <value>The default root folder.</value>
        public static string DefaultRootFolder
        {
            get
            {
                return "shared";
            }
        }

        /// <summary>
        /// Gets the default connect period millisecond.
        /// </summary>
        /// <value>The default connect period millisecond.</value>
        public static int DefaultConnectPeriodMs
        {
            get
            {
                return 1000;
            }
        }

        /// <summary>
        /// Gets the default file watcher time interval millisecond.
        /// </summary>
        /// <value>The default file watcher time interval millisecond.</value>
        public static int DefaultFileWatcherTimeIntervalMs
        {
            get
            {
                return 1000;
            }
        }

        /// <summary>
        /// Gets the size of the max request.
        /// </summary>
        /// <value>The size of the max request.</value>
        public static int MaxRequestSize
        {
            get
            {
                return 4 * 1024 * 1024;
            }       
        }

        /// <summary>
        /// Gets the weight of bandwidth.
        /// </summary>
        public static double RMBandwidthWeight
        {
            get
            {
                return 1e-5;
            }
        }

        /// <summary>
        /// Gets the weight of uniqueness.
        /// </summary>
        public static double RMUniquenessWeight
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the weight for size.
        /// </summary>
        public static double RMSizeWeight
        {
            get
            {
                return 5e-7;
            }
        }

        /// <summary>
        /// Gets the coefficient for expire.
        /// </summary>
        public static int ExpireSlackCoefficient
        {
            get
            {
                return 4;
            }
        }
    }
}
