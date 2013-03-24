using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;
using Distribox.FileSystem;

namespace Distribox.Network
{
    class VersionListMessage : ProtocolMessage
    {
		public VersionList List { get; set;}

        public VersionListMessage(VersionList list, int myPort)
            : base(myPort)
        {
            List = list;
            _type = MessageType.VersionListMessage;
        }

		public override void Accept(AntiEntropyProtocol visitor, Peer peer)
		{
			visitor.Process(this, peer);
		}
    }
}
