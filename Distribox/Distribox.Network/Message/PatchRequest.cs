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
    /// Message of patch request.
    /// </summary>
    class PatchRequest : ProtocolMessage
    {
        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request.</value>
        public List<AtomicPatch> Request { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.PatchRequest"/> class.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="myPort">My port.</param>
        public PatchRequest(List<AtomicPatch> request, int myPort)
            : base(myPort)
        {
            Request = request;
            _type = MessageType.FileRequest;
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
