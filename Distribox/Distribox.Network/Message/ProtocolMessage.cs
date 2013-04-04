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
    internal class ProtocolMessage
    {
        // FIXME: Use a factory and make this class abstract
        // FIXME: I set all of these public for JSON serialize

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.ProtocolMessage"/> class.
        /// </summary>
        /// <param name="listeningPort">Listening port.</param>
        public ProtocolMessage(int listeningPort)
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
            /// The file request.
            /// </summary>
            FileRequest,
            
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
        /// Parses to derived class.
        /// </summary>
        /// <returns>
        /// The to derived class.
        /// </returns>
        /// <param name='data'>
        /// Serialized ProtocolMessage data.
        /// </param>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public ProtocolMessage ParseToDerivedClass(byte[] data)
        {
            switch (this.Type)
            {
                case MessageType.InvitationRequest:
                    return CommonLib.CommonHelper.Deserialize<InvitationRequest>(data);
                case MessageType.InvitationAck:
                    return CommonLib.CommonHelper.Deserialize<InvitationAck>(data);
                case MessageType.SyncRequest:
                    return CommonLib.CommonHelper.Deserialize<SyncRequest>(data);
                case MessageType.SyncAck:
                    return CommonLib.CommonHelper.Deserialize<SyncAck>(data);
                case MessageType.PeerListMessage:
                    return CommonLib.CommonHelper.Deserialize<PeerListMessage>(data);
                case MessageType.VersionListMessage:
                    return CommonLib.CommonHelper.Deserialize<VersionListMessage>(data);
                case MessageType.FileRequest:
                    return CommonLib.CommonHelper.Deserialize<PatchRequest>(data);
                case MessageType.FileResponse:
                    return CommonLib.CommonHelper.Deserialize<FileDataResponse>(data);
            }

            throw new Exception("ToDerivedClass: What class is this? Maybe you forgot to add an enum / case statement?");            
        }
  
        /// <summary>
        /// Accept the specified visitor and peer.
        /// </summary>
        /// <param name='visitor'>
        /// The Visitor.
        /// </param>
        /// <param name='peer'>
        /// The Peer.
        /// </param>
        /// <exception cref='NotImplementedException'>
        /// Is thrown when a requested operation is not implemented for a given type.
        /// </exception>
        public virtual void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            // TODO avoid instaniate this class. use factory
            throw new NotImplementedException();
        }
    }
}
