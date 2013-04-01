namespace Distribox.Network
{
    /// <summary>
    /// Response of file data.
    /// </summary>
    internal class FileDataResponse : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.FileDataResponse"/> class.
        /// </summary>
        /// <param name="data">Binary Data.</param>
        /// <param name="myPort">My port.</param>
        public FileDataResponse(byte[] data, int myPort)
            : base(myPort)
        {
            this.Data = data;
            this.Type = MessageType.FileResponse;
        }

        /// <summary>
        /// Gets the binary data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data { get; set; }

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
