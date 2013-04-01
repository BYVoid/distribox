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
        public enum MessageType
        {
            InvitationRequest,
            InvitationAck,
            SyncRequest,
            SyncAck,
            PeerListMessage,
            VersionListMessage,
            FileRequest,
            FileResponse
        }

        /// <summary>
        /// Gets the listening port.
        /// </summary>
        /// <value>The listening port.</value>
        public int ListeningPort { get; set; }

        /// <summary>
        /// The type of message.
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.ProtocolMessage"/> class.
        /// </summary>
        /// <param name="listeningPort">Listening port.</param>
        public ProtocolMessage(int listeningPort)
        {
            this.ListeningPort = listeningPort;
        }

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

        public virtual void Accept(AntiEntropyProtocol visitor, Peer peer)
        {
            // TODO avoid instaniate this class. use factory
            throw new NotImplementedException();
        }
    }
}
