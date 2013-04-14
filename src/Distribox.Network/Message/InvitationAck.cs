//-----------------------------------------------------------------------
// <copyright file="InvitationAck.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    /// <summary>
    /// Acknowledgement of invitation message.
    /// </summary>
    internal class InvitationAck : ProtocolMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.InvitationAck"/> class.
        /// </summary>
        /// <param name="myListenPort">My listen port.</param>
        public InvitationAck(int myListenPort)
            : base(myListenPort)
        {
            this.Type = MessageType.InvitationAck;
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
