//-----------------------------------------------------------------------
// <copyright file="ProtocolMessageFactoryTest.cs" company="CompanyName">
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
    using System.Threading;        
    using Distribox.CommonLib;
    using Distribox.FileSystem;
    using Distribox.Network;
    using NUnit.Framework;       

    /// <summary>
    /// Test entry for ProtocolMessageFactory.
    /// </summary>
    [TestFixture]
    public class ProtocolMessageFactoryTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolMessageFactoryTest" /> class.
        /// </summary>
        public ProtocolMessageFactoryTest()
        {
            if (!Directory.Exists(".Distribox"))
            {
                Directory.CreateDirectory(".Distribox");
            }
        }

        /// <summary>
        /// Test the factory.
        /// </summary>
        /// <param name="message">The message.</param>
        public void TestFactory(ProtocolMessage message)
        {
            ProtocolMessageFactory factory = new ProtocolMessageFactory();
            ProtocolMessage message2 = factory.CreateMessage(new ProtocolMessageContainer(message));
        }

        /// <summary>
        /// Test factory. Encode then decode the ProtocolMessages.
        /// </summary>
        [Test, Timeout(100000)]
        public void Test()
        {
            InvitationRequest request = new InvitationRequest(1111);
            this.TestFactory(request);

            InvitationAck ack = new InvitationAck(2222);
            this.TestFactory(ack);

            byte[] data = { 12, 34, 56 };
            FileDataResponse dataResponse = new FileDataResponse(data, 3333);
            this.TestFactory(dataResponse);

            FileEvent e1 = new FileEvent();
            e1.Name = "1234";
            List<FileEvent> lf = new List<FileEvent>();
            lf.Add(e1);
            PatchRequest patchRequest = new PatchRequest(lf, 4444);
            this.TestFactory(patchRequest);

            PeerList peerList = PeerList.GetPeerList("abc");
            peerList.AddPeer(new Peer("127.0.0.1", 5555));
            PeerListMessage pm = new PeerListMessage(peerList, 6666);
            this.TestFactory(pm);

            SyncAck syncAck = new SyncAck(7777);
            this.TestFactory(syncAck);

            SyncRequest syncRequest = new SyncRequest(8888);
            this.TestFactory(syncRequest);

            File.WriteAllText(".Distribox/VersionList.txt", "[]");
            VersionList vl = new VersionList();
            VersionListMessage vm = new VersionListMessage(vl, 9999);
            this.TestFactory(vm);
        }
    }
}
