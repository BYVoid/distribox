using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;

namespace Distribox.Network
{
    class FileDataResponse : ProtocolMessage
    {
        public byte[] _data;

        public FileDataResponse(byte[] data, int myPort)
            : base(myPort)
        {
            _data = data;
            _type = MessageType.FileResponse;
		}
		
		public override void Accept(AntiEntropyProtocol visitor, Peer peer)
		{
			visitor.Process(this, peer);
		}
    }
}
