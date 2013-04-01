using System;

namespace Distribox.CommonLib
{
    /// <summary>
    /// Global Constants. Including string resources and default config values.
    /// </summary>
    public static class Properties
    {
        public const string PathSep = "/";
        public const string MetaFolder = ".Distribox";
        public const string MetaFolderTmp = Properties.MetaFolder + Properties.PathSep + "tmp";
        public const string MetaFolderData = Properties.MetaFolder + Properties.PathSep + "data";
        public const string BundleFileExt = ".zip";
        public const string DeltaFile = "Delta.txt";
        public const string VersionListFile = "VersionList.txt";
        public const string VersionListFilePath = Properties.MetaFolder + Properties.PathSep + Properties.VersionListFile;
        public const string PeerListFile = "PeerList.json";
        public const string PeerListFilePath = Properties.MetaFolder + Properties.PathSep + Properties.PeerListFile;
        public const string ConfigFileName = "config.json";
        public const int DefaultListenPort = 7777;
        public const string DefaultRootFolder = "shared";
        public const int DefaultConnectPeriodMs = 1000;
        public const int DefaultFileWatcherTimeIntervalMs = 1000;
        public const int MaxRequestSize = 4 * 1024 * 1024;
    }
}
