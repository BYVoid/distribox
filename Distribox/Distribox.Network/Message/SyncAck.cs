namespace Distribox.Network
{
    /// <summary>
    /// Acknowledgement of sync request.
    /// </summary>
    internal class SyncAck : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.SyncAck"/> class.
        /// </summary>
        /// <param name="myListenPort">My listen port.</param>
        public SyncAck(int myListenPort)
            : base(myListenPort)
        {
            this.Type = MessageType.SyncAck;
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
