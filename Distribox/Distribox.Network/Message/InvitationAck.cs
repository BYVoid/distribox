using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;

namespace Distribox.Network
{
    /// <summary>
    /// Acknowledgement of invitation message.
    /// </summary>
    class InvitationAck : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.InvitationAck"/> class.
        /// </summary>
        /// <param name="myListenPort">My listen port.</param>
        public InvitationAck(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.InvitationAck;
        }

        /// <summary>
        /// Accept the specified visitor and peer.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        /// <param name="peer">Peer.</param>
        public override void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            visitor.Process(this, peer);
        }
    }
}
