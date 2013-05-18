//-----------------------------------------------------------------------
// <copyright file="PeerTest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Distribox.Network;
    using NUnit.Framework;    

    /// <summary>
    /// Test entry for peer.
    /// </summary>
    [TestFixture]
    public class PeerTest
    {
        /// <summary>
        /// Test properties and hash methods.
        /// </summary>
        [Test]
        public void Test()
        {
            Peer peer = new Peer();

            string internetProtocolAddressStr1 = "127.255.0.16";
            string internetProtocolAddressStr2 = "1.2.3.4";
            int port1 = 12345678;
            int port2 = 1234;

            IPAddress address = IPAddress.Parse(internetProtocolAddressStr1);
            
            // Test Constructor
            Peer peer2 = new Peer(address, port1);
            Assert.AreEqual(internetProtocolAddressStr1, peer2.IP);
            Assert.AreEqual(port1, peer2.Port);

            Peer peer3 = new Peer(internetProtocolAddressStr1, port1);
            Assert.AreEqual(internetProtocolAddressStr1, peer3.IP);
            Assert.AreEqual(port1, peer3.Port);

            Assert.IsTrue(peer2.Equals(peer3));
            Assert.IsTrue(peer3.Equals(peer2));
            Assert.AreEqual(peer2.GetHashCode(), peer3.GetHashCode());

            peer2.IP = internetProtocolAddressStr2;            
            Assert.IsFalse(peer2.Equals(peer3));
            Assert.IsFalse(peer3.Equals(peer2));
            Assert.AreNotEqual(peer2.GetHashCode(), peer3.GetHashCode());

            peer2.IP = internetProtocolAddressStr1;
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
