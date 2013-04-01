using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;

namespace Distribox.Network
{
    /// <summary>
    /// Response of file data.
    /// </summary>
    class FileDataResponse : ProtocolMessage
    {
        /// <summary>
        /// Gets the binary data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.FileDataResponse"/> class.
        /// </summary>
        /// <param name="data">Binary Data.</param>
        /// <param name="myPort">My port.</param>
        public FileDataResponse(byte[] data, int myPort)
            : base(myPort)
        {
            Data = data;
            _type = MessageType.FileResponse;
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
