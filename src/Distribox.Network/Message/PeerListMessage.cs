//-----------------------------------------------------------------------
// <copyright file="PeerListMessage.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    /// <summary>
    /// Message of propagating peer list.
    /// </summary>
    public class PeerListMessage : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.PeerListMessage"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="port">The port.</param>
        public PeerListMessage(PeerList list, int port) : base(port)
        {
            this.List = list;
            this.Type = MessageType.PeerListMessage;
        }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        public PeerList List { get; set; }

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
