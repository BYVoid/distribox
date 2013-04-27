//-----------------------------------------------------------------------
// <copyright file="ProtocolMessageFactory.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using System;
    using Distribox.CommonLib;

    /// <summary>
    /// Factory to create ProtocolMessages. 
    /// <para />
    /// Currently, it is used only for converting ProtocolMessageContainer to ProtocolMessage.
    /// </summary>
    internal class ProtocolMessageFactory
    {
        /// <summary>
        /// Convert <paramref name="container"/> to ProtocolMessage.
        /// </summary>
        /// <param name="container">The container to be converted.</param>
        /// <returns>The converted protocol message.</returns>
        public ProtocolMessage CreateMessage(ProtocolMessageContainer container)
        {
            byte[] data = container.Data;
            switch (container.Type)
            {
                case ProtocolMessage.MessageType.InvitationRequest:
                    return CommonHelper.Deserialize<InvitationRequest>(data);
                case ProtocolMessage.MessageType.InvitationAck:
                    return CommonHelper.Deserialize<InvitationAck>(data);
                case ProtocolMessage.MessageType.SyncRequest:
                    return CommonHelper.Deserialize<SyncRequest>(data);
                case ProtocolMessage.MessageType.SyncAck:
                    return CommonHelper.Deserialize<SyncAck>(data);
                case ProtocolMessage.MessageType.PeerListMessage:
                    return CommonHelper.Deserialize<PeerListMessage>(data);
                case ProtocolMessage.MessageType.VersionListMessage:
                    return CommonHelper.Deserialize<VersionListMessage>(data);
                case ProtocolMessage.MessageType.PatchRequest:
                    return CommonHelper.Deserialize<PatchRequest>(data);
                case ProtocolMessage.MessageType.FileResponse:
                    return CommonHelper.Deserialize<FileDataResponse>(data);
            }

            throw new Exception("ToDerivedClass: What class is this? Maybe you forgot to add an enum / case statement?");
        }
    }
}
