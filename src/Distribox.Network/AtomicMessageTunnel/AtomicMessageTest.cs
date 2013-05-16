namespace Distribox.Network.AtomicMessageTunnel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Distribox.CommonLib;
    using NUnit.Framework;

    [TestFixture]
    class AtomicMessageTest
    {
        private byte[] lastData;
        private Peer lastPeerFrom;
        private bool received;
        private bool completed;

        private void OnReceiveHandler(byte[] data, Peer peerFrom)
        {           
            Assert.AreEqual(CommonHelper.ByteToString(this.lastData), 
                            CommonHelper.ByteToString(data));

            Assert.AreEqual(this.lastPeerFrom.IP, peerFrom.IP);

            this.received = true;
        }

        private void OnCompleteHandler(Exception err)
        {
            Assert.IsTrue(this.received);
            this.completed = true;
        }

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
            this.completed = false;
            sender.SendBytes(message);

            Thread.Sleep(3);
        }
    }
}
