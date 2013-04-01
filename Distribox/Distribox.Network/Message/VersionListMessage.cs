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
        /// <param name="list">List.</param>
        /// <param name="myPort">My port.</param>
        public VersionListMessage(VersionList list, int myPort)
            : base(myPort)
        {
            this.List = list;
            this.Type = MessageType.VersionListMessage;
        }

        /// <summary>
        /// Gets the version list.
        /// </summary>
        /// <value>The list.</value>
        public VersionList List { get; set; }

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
