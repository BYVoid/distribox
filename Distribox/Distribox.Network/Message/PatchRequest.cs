namespace Distribox.Network
{
    using System.Collections.Generic;
    using Distribox.FileSystem;

    /// <summary>
    /// Message of patch request.
    /// </summary>
    internal class PatchRequest : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.PatchRequest"/> class.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="myPort">My port.</param>
        public PatchRequest(List<FileEvent> request, int myPort)
            : base(myPort)
        {
            this.Request = request;
            this.Type = MessageType.FileRequest;
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request.</value>
        public List<FileEvent> Request { get; set; }

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
