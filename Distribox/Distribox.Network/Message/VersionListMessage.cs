using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;
using Distribox.FileSystem;

namespace Distribox.Network
{
    /// <summary>
    /// Message of propagating version list.
    /// </summary>
    class VersionListMessage : ProtocolMessage
    {
        /// <summary>
        /// Gets the version list.
        /// </summary>
        /// <value>The list.</value>
        public VersionList List { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.VersionListMessage"/> class.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="myPort">My port.</param>
        public VersionListMessage(VersionList list, int myPort)
            : base(myPort)
        {
            List = list;
            _type = MessageType.VersionListMessage;
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
