//-----------------------------------------------------------------------
// <copyright file="SyncRequest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Message of sync request.
    /// </summary>
    public class SyncRequest : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.SyncRequest"/> class.
        /// </summary>
        /// <param name="myListenPort">My listen port.</param>
        public SyncRequest(int myListenPort)
            : base(myListenPort)
        {
            this.Type = MessageType.SyncRequest;
        }

        /// <summary>
        /// Accept the specified visitor and peer.
        /// </summary>
        /// <param name="visitor">The Visitor.</param>
        /// <param name="peer">The Peer.</param>
        /// <exception cref="NotImplementedException">Is thrown when a requested operation is not implemented for a given type.</exception>
        public override void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            visitor.Process(this, peer);
        }
    }
}
