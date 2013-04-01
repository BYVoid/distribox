namespace Distribox.Network
{
    /// <summary>
    /// Message of sync request.
    /// </summary>
    internal class SyncRequest : ProtocolMessage
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
        /// <param name="visitor">Visitor.</param>
        /// <param name="peer">Peer.</param>
        public override void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            visitor.Process(this, peer);
        }
    }
}
