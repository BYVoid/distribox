namespace Distribox.Network.Message
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Distribox.CommonLib;
    using NUnit.Framework;

    [TestFixture]
    class ProtocolMessageContainerTest
    {
        [Test]
        public void Test()
        {
            InvitationRequest request = new InvitationRequest(1234);

            ProtocolMessageContainer container = new ProtocolMessageContainer(request);

            Assert.AreEqual(ProtocolMessage.MessageType.InvitationRequest, container.Type);
            Assert.AreEqual(request.SerializeAsBytes(), container.Data);
        }
    }
}
