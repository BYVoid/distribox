using Distribox.CommonLib;
using Distribox.FileSystem;
using Distribox.Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Distribox.Test
{
    [TestFixture]
    class SingleNodeTest
    {
        private AntiEntropyProtocol protocol;

        [Test]
        public void Test()
        {
            if (File.Exists("config.json"))
            {
                File.Delete("config.json");
            }
            if (File.Exists("Haha"))
            {
                File.Delete("Haha");
            }
            if (Directory.Exists("Dir"))
            {
                Directory.Delete("Dir", true);
            }
            if (Directory.Exists(".Distribox"))
            {
                Directory.Delete(".Distribox", true);
            }

            // Get config
            int port = Config.ListenPort;

            // Initialize folder
            CommonHelper.InitializeFolder();

            StartPeer(7777);

            // Create File
            File.WriteAllText("Haha", "test");

            // Change File
            File.WriteAllText("Haha", "test1");

            // Change File
            File.WriteAllText("Haha", "test2");

            // Rename File
            File.Move("Haha", "XX");

            // Delete File
            File.Delete("XX");

            // Create Directory
            Directory.CreateDirectory("Dir");

            Thread.Sleep(1000);
        }

        private void StartPeer(int port)
        {
            string peerListName = Config.PeerListFilePath;

            // Initialize anti entropy protocol
            var vs = new VersionControl();
            this.protocol = new AntiEntropyProtocol(port, peerListName, vs);

            // Initialize file watcher
            FileWatcher watcher = new FileWatcher();
            watcher.Created += vs.Created;
            watcher.Changed += vs.Changed;
            watcher.Deleted += vs.Deleted;
            watcher.Renamed += vs.Renamed;
            watcher.Idle += vs.Flush;
        }
    }
}
