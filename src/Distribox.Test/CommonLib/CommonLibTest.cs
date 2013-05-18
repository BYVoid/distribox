﻿//-----------------------------------------------------------------------
// <copyright file="CommonLibTest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Distribox.CommonLib;
    using NUnit.Framework;
    
    /// <summary>
    /// Test entry for CommonLib.
    /// </summary>
    [TestFixture]
    public class CommonLibTest
    {
        /// <summary>
        /// Test logger.
        /// </summary>
        [Test]
        public void LoggerTest()
        {
            Logger.Info("Info");
            Logger.Warn("Warn");
            Logger.Debug("Debug");
            Logger.Error("Error");
            Logger.Fatal("Fatal");
        }

        /// <summary>
        /// Test serialization.
        /// </summary>
        [Test]
        public void SerializeTest()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["asd"] = "dsa";
            string str = dict.Serialize();
            string str_inline = dict.SerializeInline();
            byte[] bytes = dict.SerializeAsBytes();
            str.Deserialize<Dictionary<string, string>>();
            bytes.Deserialize<Dictionary<string, string>>();
        }

        /// <summary>
        /// Test properties.
        /// </summary>
        [Test]
        public void PropertiesTest()
        {
            Assert.AreEqual(Properties.PathSep, "/");
            Assert.AreEqual(Properties.MetaFolder, ".Distribox");
            Assert.AreEqual(Properties.MetaFolderTmp, "tmp");
            Assert.AreEqual(Properties.MetaFolderData, "data");
            Assert.AreEqual(Properties.BundleFileExt, ".zip");
            Assert.AreEqual(Properties.DeltaFile, "Delta.txt");
            Assert.AreEqual(Properties.VersionListFile, "VersionList.txt");
            Assert.AreEqual(Properties.PeerListFile, "PeerList.json");
            Assert.AreEqual(Properties.PeerBandwidthFile, "PeerBandwidth.json");
            Assert.AreEqual(Properties.ConfigFileName, "config.json");
            Assert.AreEqual(Properties.DefaultListenPort, 7777);
            Assert.AreEqual(Properties.DefaultBandwidth, 10 * 1024 * 1024);
            Assert.AreEqual(Properties.DefaultDelay, 3);
            Assert.AreEqual(Properties.DefaultConnectionSpeed, 20 * 1024);
            Assert.AreEqual(Properties.DefaultRootFolder, "shared");
            Assert.AreEqual(Properties.DefaultConnectPeriodMs, 1000);
            Assert.AreEqual(Properties.DefaultFileWatcherTimeIntervalMs, 1000);
            Assert.AreEqual(Properties.MaxRequestSize, 4 * 1024 * 1024);
            Assert.AreEqual(Properties.RMBandwidthWeight, 1e-5);
            Assert.AreEqual(Properties.RMUniquenessWeight, 1);
            Assert.AreEqual(Properties.RMSizeWeight, 5e-7);
            Assert.AreEqual(Properties.ExpireSlackCoefficient, 4);
        }

        /// <summary>
        /// Test config.
        /// </summary>
        [Test]
        public void ConfigDataTest()
        {
            ConfigData data = new ConfigData();
            data.SetDefault();
        }

        /// <summary>
        /// Test stream.
        /// </summary>
        [Test]
        public void StreamTest()
        {
            MemoryStream stream = new MemoryStream();
            stream.WriteAllBytes(CommonHelper.StringToByte("Hello"));
            stream.Position = 0;
            var bytes = stream.ReadAllBytes();
            Assert.AreEqual(CommonHelper.ByteToString(bytes), "Hello");
        }

        /// <summary>
        /// Test hash.
        /// </summary>
        [Test]
        public void GetHashCodeTest()
        {
            List<int> list = new List<int> { 1, 2, 3 };
            int code = CommonHelper.GetHashCode(list);
        }

        /// <summary>
        /// Test default connection speed.
        /// </summary>
        [Test]
        public void DefaultConnectionSpeedTest()
        {
            Assert.IsTrue(Config.DefaultConnectionSpeed >= 0);
        }
    }
}
