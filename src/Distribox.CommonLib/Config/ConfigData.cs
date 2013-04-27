//-----------------------------------------------------------------------
// <copyright file="ConfigData.cs" company="CompanyName">
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
    /// Config data.
    /// </summary>
    public class ConfigData
    {
        /// <summary>
        /// Gets or sets the listen port.
        /// </summary>
        /// <value>The listen port.</value>
        public int ListenPort { get; set; }

        public int DefaultBandwidth { get; set; }

        public int DefaultConnectionSpeed { get; set; }

        /// <summary>
        /// Gets or sets the connect period millisecond.
        /// </summary>
        /// <value>The connect period millisecond.</value>
        public int ConnectPeriodMs { get; set; }

        /// <summary>
        /// Gets or sets the file watcher time interval millisecond.
        /// </summary>
        /// <value>The file watcher time interval millisecond.</value>
        public int FileWatcherTimeIntervalMs { get; set; }

        /// <summary>
        /// Gets or sets the root folder.
        /// </summary>
        /// <value>The root folder.</value>
        public string RootFolder { get; set; }

        /// <summary>
        /// Sets the default.
        /// </summary>
        public void SetDefault()
        {
            this.ListenPort = Properties.DefaultListenPort;

            // Use working directory/shared as root folder
            this.RootFolder = Directory.GetCurrentDirectory();

            this.ConnectPeriodMs = Properties.DefaultConnectPeriodMs;

            this.FileWatcherTimeIntervalMs = Properties.DefaultFileWatcherTimeIntervalMs;

            this.DefaultBandwidth = Properties.DefaultBandwidth;
            this.DefaultConnectionSpeed = Properties.DefaultConnectionSpeed;

            CommonHelper.WriteObject(this, Properties.ConfigFileName);
        }
    }
}
