namespace Distribox.Network.Message
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Distribox.CommonLib;
    using Distribox.FileSystem;
    using NUnit.Framework;    

    [TestFixture]
    class ProtocolMessageFactoryTest
    {

        public void TestFactory(ProtocolMessage message)
        {
            ProtocolMessageFactory factory = new ProtocolMessageFactory();
            ProtocolMessage message2 = factory.CreateMessage(new ProtocolMessageContainer(message));
            Assert.AreEqual(message.SerializeInline(), message2.SerializeInline());
        }

        [Test]
        public void Test()
        {
            InvitationRequest request = new InvitationRequest(1111);
            this.TestFactory(request);

            InvitationAck ack = new InvitationAck(2222);
            this.TestFactory(ack);

            byte[] data = {12, 34, 56};
            FileDataResponse dataResponse = new FileDataResponse(data, 3333);
            this.TestFactory(dataResponse);

            FileEvent e1 = new FileEvent();
            e1.Name = "1234";
            List<FileEvent> lf = new List<FileEvent>();
            lf.Add(e1);
            PatchRequest pRequest = new PatchRequest(lf, 4444);
            this.TestFactory(pRequest);

            PeerList pList = PeerList.GetPeerList("abc");
            pList.AddPeer(new Peer("127.0.0.1", 5555));
            PeerListMessage pm = new PeerListMessage(pList, 6666);
            this.TestFactory(pm);

            SyncAck syncAck = new SyncAck(7777);
            this.TestFactory(syncAck);

            SyncRequest syncRequest = new SyncRequest(8888);
            this.TestFactory(syncRequest);

            VersionList vl = new VersionList();
            VersionListMessage vm = new VersionListMessage(vl, 9999);
            this.TestFactory(vm);
        }
    }
}
