using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribox.Network
{
    class FileRequest : ProtocolMessage
    {
        public List<CommonLib.FileItem> _request;

        public FileRequest(List<CommonLib.FileItem> request, int myPort)
            : base(myPort)
        {
            _request = request;
            _type = MessageType.FileRequest;
		}
		
		public override void Accept(AntiEntropyProtocol visitor, Peer peer)
		{
			visitor.Process(this, peer);
		}
    }
}
