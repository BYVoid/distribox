//-----------------------------------------------------------------------
// <copyright file="InvitationRequest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    /// <summary>
    /// Message of invitation request.
    /// </summary>
    internal class InvitationRequest : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.InvitationRequest"/> class.
        /// </summary>
        /// <param name="myListenPort">My listen port.</param>
        public InvitationRequest(int myListenPort)
            : base(myListenPort)
        {
            this.Type = MessageType.InvitationRequest;
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
