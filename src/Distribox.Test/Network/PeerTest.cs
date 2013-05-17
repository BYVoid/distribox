namespace Distribox.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using NUnit.Framework;
    using Distribox.Network;

    [TestFixture]
    public class PeerTest
    {
        [Test]
        public void Test()
        {
            Peer peer = new Peer();

            string ipStr1 = "127.255.0.16";
            string ipStr2 = "1.2.3.4";
            int port1 = 12345678;
            int port2 = 1234;

            IPAddress address = IPAddress.Parse(ipStr1);
            
            // Test Constructor
            Peer peer2 = new Peer(address, port1);
            Assert.AreEqual(ipStr1, peer2.IP);
            Assert.AreEqual(port1, peer2.Port);

            Peer peer3 = new Peer(ipStr1, port1);
            Assert.AreEqual(ipStr1, peer3.IP);
            Assert.AreEqual(port1, peer3.Port);

            Assert.IsTrue(peer2.Equals(peer3));
            Assert.IsTrue(peer3.Equals(peer2));
            Assert.AreEqual(peer2.GetHashCode(), peer3.GetHashCode());

            peer2.IP = ipStr2;            
            Assert.IsFalse(peer2.Equals(peer3));
            Assert.IsFalse(peer3.Equals(peer2));
            Assert.AreNotEqual(peer2.GetHashCode(), peer3.GetHashCode());

            peer2.IP = ipStr1;
            Assert.IsTrue(peer2.Equals(peer3));
            Assert.IsTrue(peer3.Equals(peer2));
            Assert.AreEqual(peer2.GetHashCode(), peer3.GetHashCode());

            peer3.Port = port2;
            Assert.IsFalse(peer2.Equals(peer3));
            Assert.IsFalse(peer3.Equals(peer2));
            Assert.AreNotEqual(peer2.GetHashCode(), peer3.GetHashCode());
        }        
    }
}
