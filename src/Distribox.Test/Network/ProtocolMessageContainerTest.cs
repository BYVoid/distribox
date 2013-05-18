//-----------------------------------------------------------------------
// <copyright file="ProtocolMessageContainerTest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//---------------------------------------------------------------------
namespace Distribox.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Distribox.CommonLib;    
    using Distribox.Network;
    using NUnit.Framework;

    /// <summary>
    /// Test entry for ProtocolMessageContainer.
    /// </summary>
    [TestFixture]
    public class ProtocolMessageContainerTest
    {
        /// <summary>
        /// Test the encoding.
        /// </summary>
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
