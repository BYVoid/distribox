//-----------------------------------------------------------------------
// <copyright file="PatchRequest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
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
        /// <param name="request">The request.</param>
        /// <param name="myPort">My port.</param>
        public PatchRequest(List<FileEvent> request, int myPort)
            : base(myPort)
        {
            this.Request = request;
            this.Type = MessageType.PatchRequest;
        }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public List<FileEvent> Request { get; set; }

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
