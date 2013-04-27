//-----------------------------------------------------------------------
// <copyright file="FileDataResponse.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
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
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data { get; set; }

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
