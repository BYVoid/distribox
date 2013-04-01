using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;

namespace Distribox.Network
{
    class SyncAck : ProtocolMessage
    {
        public SyncAck(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.SyncAck;
        }
        
        public override void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            visitor.Process(this, peer);
        }
    }
}
