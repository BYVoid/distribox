//-----------------------------------------------------------------------
// <copyright file="PeerListTest.cs" company="CompanyName">
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
    using Distribox.Network;
    using NUnit.Framework;
    
    /// <summary>
    /// Test class for PeerList.
    /// </summary>
    [TestFixture]
    public class PeerListTest
    {
        /// <summary>
        /// Test add, merge.
        /// </summary>
        [Test]
        public void TestOperation()
        {
            string fileName = "peerList.test.tmp";
            string fileName2 = "peerList.test.tmp2";
            PeerList peerList = new PeerList(fileName);

            Assert.AreEqual(fileName, peerList.PeerFileName);
            Assert.AreEqual(0, peerList.Peers.Count);

            Peer peer1 = new Peer("127.0.0.1", 8888);
            Peer peer2 = new Peer("127.0.0.2", 8888);
            Peer peer3 = new Peer("127.0.0.2", 9999);

            peerList.AddPeer(peer1);
            Assert.AreEqual(1, peerList.Peers.Count);

            peerList.AddPeer(peer1);
            Assert.AreEqual(1, peerList.Peers.Count);
            Assert.IsTrue(peerList.Peers.Contains(peer1));
            Assert.IsFalse(peerList.Peers.Contains(peer2));

            // Test FileOperation
            PeerList peerList2 = PeerList.GetPeerList(fileName);
            Assert.AreEqual(1, peerList.Peers.Count);
            Assert.IsTrue(peerList.Peers.Contains(peer1));
            Assert.IsFalse(peerList.Peers.Contains(peer2));

            peerList2.AddPeer(peer2);

            Assert.IsFalse(peerList.Peers.Contains(peer2));
            peerList = PeerList.GetPeerList(fileName);
            Assert.IsTrue(peerList.Peers.Contains(peer2));

            // Test Merge
            PeerList peerList3 = PeerList.GetPeerList(fileName2);
            Assert.AreEqual(0, peerList3.Peers.Count);
            peerList3.AddPeer(peer3);
            Assert.AreEqual(1, peerList3.Peers.Count);

            peerList3.MergeWith(peerList2);
            Assert.IsTrue(peerList3.Peers.Contains(peer1));
            Assert.IsTrue(peerList3.Peers.Contains(peer2));
            Assert.IsTrue(peerList3.Peers.Contains(peer3));

            int peer1Cnt = 0, peer2Cnt = 0;
            for (int i = 0; i < 10000; ++i)
            {
                Peer peer = peerList2.SelectRandomPeer();

                Assert.IsTrue(peerList2.Peers.Contains(peer));
                Assert.IsTrue(peer.Equals(peer1) || peer.Equals(peer2));

                if (peer.Equals(peer1))
                {
                    ++peer1Cnt;
                }
                else
                {
                    ++peer2Cnt;
                }
            }

            Assert.Less((double)(Math.Abs(peer1Cnt - peer2Cnt) / 10000), 0.05);

            // Clean up
            File.Delete(fileName);
            File.Delete(fileName2);
        }
    }
}
