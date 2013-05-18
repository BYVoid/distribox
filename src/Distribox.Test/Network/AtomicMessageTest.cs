// <copyright file="AtomicMessageTest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Distribox.CommonLib;    
    using Distribox.Network;
    using NUnit.Framework;

    /// <summary>
    /// Test class for AtomicMessage.
    /// </summary>
    [TestFixture]
    public class AtomicMessageTest
    {
        /// <summary>
        /// Remembers what I send last time.
        /// </summary>
        private byte[] lastData;
        
        /// <summary>
        /// Remembers whom I receive from last time.
        /// </summary>
        private Peer lastPeerFrom;
        
        /// <summary>
        /// See if I have received a message.
        /// </summary>
        private bool received;

        /// <summary>
        /// Test entry for AtomicMessage.
        /// </summary>
        [Test]
        public void Test()
        {
            AtomicMessageListener listener = new AtomicMessageListener(1234);
            listener.OnReceive += this.OnReceiveHandler;

            AtomicMessageSender sender = new AtomicMessageSender(new Peer("127.0.0.1", 1234));

            byte[] message = { 1, 2, 43, 23 };
            this.lastData = message;
            this.lastPeerFrom = new Peer("127.0.0.1", -1);
            this.received = false;
            sender.SendBytes(message);

            Thread.Sleep(3);
        }

        /// <summary>
        /// Received handler for listener.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="peerFrom">The peer.</param>
        private void OnReceiveHandler(byte[] data, Peer peerFrom)
        {           
            Assert.AreEqual(
                CommonHelper.ByteToString(this.lastData), 
                CommonHelper.ByteToString(data));

            Assert.AreEqual(this.lastPeerFrom.IP, peerFrom.IP);

            this.received = true;
        }

        /// <summary>
        /// Complete handler for the sender.
        /// </summary>
        /// <param name="err">The error.</param>
        private void OnCompleteHandler(Exception err)
        {
            Assert.IsTrue(this.received);
        }        
    }
}
