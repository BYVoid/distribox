//-----------------------------------------------------------------------
// <copyright file="VersionListMessage.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using Distribox.FileSystem;

    /// <summary>
    /// Message of propagating version list.
    /// </summary>
    internal class VersionListMessage : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.VersionListMessage"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="myPort">My port.</param>
        public VersionListMessage(VersionList list, int myPort)
            : base(myPort)
        {
            this.List = list;
            this.Type = MessageType.VersionListMessage;
        }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        public VersionList List { get; set; }

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
