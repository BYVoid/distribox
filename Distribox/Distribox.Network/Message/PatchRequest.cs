using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;
using Distribox.FileSystem;

namespace Distribox.Network
{
    class PatchRequest : ProtocolMessage
    {
        public List<AtomicPatch> Request { get; set; }

        public PatchRequest(List<AtomicPatch> request, int myPort)
            : base(myPort)
        {
            Request = request;
            _type = MessageType.FileRequest;
        }
        
        public override void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            visitor.Process(this, peer);
        }
    }
}
