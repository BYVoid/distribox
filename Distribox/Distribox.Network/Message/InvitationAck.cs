using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribox.Network
{
    class InvitationAck : ProtocolMessage
    {
        public InvitationAck(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.InvitationAck;
		}
		
		public override void Accept(AntiEntropyProtocol visitor, Peer peer)
		{
			visitor.Process(this, peer);
		}
    }
}
