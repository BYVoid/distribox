using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;

namespace Distribox.Network
{
    class InvitationRequest : ProtocolMessage
    {
        public InvitationRequest(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.InvitationRequest;
        }
        
        public override void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            visitor.Process(this, peer);
        }
    }
}
