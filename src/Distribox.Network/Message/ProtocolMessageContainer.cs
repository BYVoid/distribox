//-----------------------------------------------------------------------
// <copyright file="ProtocolMessageContainer.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using System;
    using Distribox.CommonLib;

    /// <summary>
    /// Contains the type and serialized data of a derived class of ProtocolMessage.
	/// 
    /// Type is for determination of how to deserialize the contained ProtocolMessage.
    /// </summary>
    internal class ProtocolMessageContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolMessageContainer"/> class.
        /// Create a container from ProtocolMessage.
        /// </summary>
        /// <param name="message">The message.</param>
        public ProtocolMessageContainer(ProtocolMessage message)
        {
            this.Type = message.Type;
            this.Data = message.SerializeAsBytes();
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public ProtocolMessage.MessageType Type { get; set; }

        /// <summary>
        /// Gets or sets the serialized data of the real ProtocolMessage.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
