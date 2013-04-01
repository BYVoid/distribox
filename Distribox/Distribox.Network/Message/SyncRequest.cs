using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;

namespace Distribox.Network
{
    class SyncRequest : ProtocolMessage
    {
        public SyncRequest(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.SyncRequest;
        }
        
        public override void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            visitor.Process(this, peer);
        }
    }
}
