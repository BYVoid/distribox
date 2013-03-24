using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;
using Distribox.FileSystem;

namespace Distribox.Network
{
    class FileRequest : ProtocolMessage
    {
		public List<FileItem> Request { get; set; }

        public FileRequest(List<FileItem> request, int myPort)
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
