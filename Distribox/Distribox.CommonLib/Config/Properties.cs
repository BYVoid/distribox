using System;

namespace Distribox.CommonLib
{
    /// <summary>
    /// Global Constants. Including string resources and default config values.
    /// </summary>
    public static class Properties
    {
        public static string PathSep = "/";
        public static string MetaFolder = ".Distribox";
        public static string MetaFolderTmp = "tmp";
        public static string MetaFolderData = "data";
        public static string BundleFileExt = ".zip";
        public static string DeltaFile = "Delta.txt";
        public static string VersionListFile = "VersionList.txt";
        public static string PeerListFile = "PeerList.json";
        public static string ConfigFileName = "config.json";
        public static int DefaultListenPort = 7777;
        public static string DefaultRootFolder = "shared";
        public static int DefaultConnectPeriodMs = 1000;
        public static int DefaultFileWatcherTimeIntervalMs = 1000;
        public static int MaxRequestSize = 4 * 1024 * 1024;
    }
}
