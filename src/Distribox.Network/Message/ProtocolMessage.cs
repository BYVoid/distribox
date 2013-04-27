//-----------------------------------------------------------------------
// <copyright file="ProtocolMessage.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using System;

    /// <summary>
    /// Abstract message of Anti-Entropy Protocol.
    /// </summary>
    internal abstract class ProtocolMessage
    {
        // FIXME: Use a factory and make this class abstract
        // FIXME: I set all of these public for JSON serialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.ProtocolMessage"/> class.
        /// </summary>
        /// <param name="listeningPort">Listening port.</param>
        protected ProtocolMessage(int listeningPort)
        {
            this.ListeningPort = listeningPort;
        }
        
        /// <summary>
        /// Message type;
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// The invitation request.
            /// </summary>
            InvitationRequest,
            
            /// <summary>
            /// The invitation ACK.
            /// </summary>
            InvitationAck,
            
            /// <summary>
            /// The sync request.
            /// </summary>
            SyncRequest,
            
            /// <summary>
            /// The sync ACK.
            /// </summary>
            SyncAck,
            
            /// <summary>
            /// The peer list message.
            /// </summary>
            PeerListMessage,
            
            /// <summary>
            /// The version list message.
            /// </summary>
            VersionListMessage,
            
            /// <summary>
            /// The patch request.
            /// </summary>
            PatchRequest,
            
            /// <summary>
            /// The file response.
            /// </summary>
            FileResponse
        }
        
        /// <summary>
        /// Gets or sets the listening port.
        /// </summary>
        /// <value>The listening port.</value>
        public int ListeningPort { get; set; }
        
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public MessageType Type { get; set; }
  
        /// <summary>
        /// Accept the specified visitor and peer.
        /// </summary>
        /// <param name='visitor'>
        /// The Visitor.
        /// </param>
        /// <param name='peer'>
        /// The Peer.
        /// </param>
        public abstract void Accept(AntiEntropyProtocol visitor, Peer peer);
    }
}
