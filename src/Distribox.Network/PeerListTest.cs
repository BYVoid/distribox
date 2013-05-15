namespace Distribox.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    class PeerListTest
    {
        [Test]
        public void TestOperation()
        {
            string fileName = "peerList.test.tmp";
            string fileName2 = "peerList.test.tmp2";
            PeerList pList = new PeerList(fileName);

            Assert.AreEqual(fileName, pList.PeerFileName);
            Assert.AreEqual(0, pList.Peers.Count);

            Peer peer1 = new Peer("127.0.0.1", 8888);
            Peer peer2 = new Peer("127.0.0.2", 8888);
            Peer peer3 = new Peer("127.0.0.2", 9999);

            pList.AddPeer(peer1);
            Assert.AreEqual(1, pList.Peers.Count);

            pList.AddPeer(peer1);
            Assert.AreEqual(1, pList.Peers.Count);
            Assert.IsTrue(pList.Peers.Contains(peer1));
            Assert.IsFalse(pList.Peers.Contains(peer2));
            
            // Test FileOperation
            PeerList pList2 = PeerList.GetPeerList(fileName);
            Assert.AreEqual(1, pList.Peers.Count);
            Assert.IsTrue(pList.Peers.Contains(peer1));
            Assert.IsFalse(pList.Peers.Contains(peer2));

            pList2.AddPeer(peer2);

            Assert.IsFalse(pList.Peers.Contains(peer2));
            pList = PeerList.GetPeerList(fileName);
            Assert.IsTrue(pList.Peers.Contains(peer2));

            // Test Merge
            PeerList pList3 = PeerList.GetPeerList(fileName2);
            Assert.AreEqual(0, pList3.Peers.Count);
            pList3.AddPeer(peer3);
            Assert.AreEqual(1, pList3.Peers.Count);

            pList3.MergeWith(pList2);
            Assert.IsTrue(pList3.Peers.Contains(peer1));
            Assert.IsTrue(pList3.Peers.Contains(peer2));
            Assert.IsTrue(pList3.Peers.Contains(peer3));

            int peer1Cnt = 0, peer2Cnt = 0;
            for (int i = 0; i < 10000; ++i)
            {
                Peer peer = pList2.SelectRandomPeer();

                Assert.IsTrue(pList2.Peers.Contains(peer));
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
