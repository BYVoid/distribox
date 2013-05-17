using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Test
{
    using System.IO;
    using System.Threading;
    using NUnit.Framework;
    using Distribox.CommonLib;
    using Distribox.Network;

    [TestFixture]
    public class BandwidthEstimatorTest
    {
        [Test, Timeout(100000)]
        public void SinglePeer()
        {
            if (File.Exists(Config.PeerBandwidthFilePath))
            {
                File.Delete(Config.PeerBandwidthFilePath);
            }

            BandwidthEstimator estimator = new BandwidthEstimator();
            Peer peer1 = new Peer("127.0.0.1", 1111);

            // Invalid peer
            Assert.AreEqual(Properties.DefaultConnectionSpeed, estimator.GetPeerBandwidth(peer1));

            // Do a task
            estimator.BeginRequest(peer1, 0x1234, 1024 * 1024);
            Thread.Sleep(1150);
            estimator.FinishRequest(0x1234);

            Assert.AreEqual(1024 * 1024, estimator.GetPeerBandwidth(peer1));

            // Do another slower task
            estimator.BeginRequest(peer1, 0x1234, 512 * 1024);
            Thread.Sleep(1150);
            estimator.FinishRequest(0x1234);

            Assert.AreEqual(512 * 1024, estimator.GetPeerBandwidth(peer1));

            // Do two tasks
            estimator.BeginRequest(peer1, 0x1234, 1024 * 1024);
            Thread.Sleep(1150);
            estimator.BeginRequest(peer1, 0x1235, 768 * 1024);
            Assert.AreEqual(512 * 1024, estimator.GetPeerBandwidth(peer1));
            estimator.FinishRequest(0x1235);
            Assert.AreEqual(768 * 1024, estimator.GetPeerBandwidth(peer1));
            Thread.Sleep(3150);
            Assert.AreEqual(768 * 1024, estimator.GetPeerBandwidth(peer1));
            estimator.FinishRequest(0x1234);
            Assert.AreEqual(256 * 1024, estimator.GetPeerBandwidth(peer1));
        
            // Fail a task
            estimator.BeginRequest(peer1, 0x1234, 1024 * 1024);
            estimator.FailRequest(0x1234);
            //Assert.AreEqual(0 * 1024, estimator.GetPeerBandwidth(peer1));
        }

        [Test, Timeout(100000)]
        public void Restore()
        {
            if (File.Exists(Config.PeerBandwidthFilePath))
            {
                File.Delete(Config.PeerBandwidthFilePath);
            }

            BandwidthEstimator estimator = new BandwidthEstimator();
            Peer peer1 = new Peer("127.0.0.1", 1111);

            estimator.BeginRequest(peer1, 0x1234, 1024 * 1024);
            Thread.Sleep(1150);
            estimator.FinishRequest(0x1234);

            BandwidthEstimator estimator2 = new BandwidthEstimator();
            Assert.AreEqual(1024 * 1024, estimator2.GetPeerBandwidth(peer1));
        }

        [Test, Timeout(100000)]
        public void MultiPeer()
        {
            if (File.Exists(Config.PeerBandwidthFilePath))
            {
                File.Delete(Config.PeerBandwidthFilePath);
            }

            BandwidthEstimator estimator = new BandwidthEstimator();
            Peer peer1 = new Peer("127.0.0.1", 1111);
            Peer peer2 = new Peer("127.0.0.1", 1111);

            estimator.BeginRequest(peer1, 0x1234, 1024 * 1024);
            estimator.BeginRequest(peer2, 0x1235, 2048 * 1024);
            estimator.FinishRequest(0x1234);
            Assert.AreEqual(1024 * 1024, estimator.TotalBandwidth);
            estimator.FinishRequest(0x1235);
            Assert.AreEqual(3072 * 1024, estimator.TotalBandwidth);
        }
    }
}
