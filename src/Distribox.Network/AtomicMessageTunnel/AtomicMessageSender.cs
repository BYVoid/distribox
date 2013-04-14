//-----------------------------------------------------------------------
// <copyright file="AtomicMessageSender.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using Distribox.CommonLib;

    /// <summary>
    /// Sender of messages to a specified peer.
    /// </summary>
    internal class AtomicMessageSender
    {
        /// <summary>
        /// The client.
        /// </summary>
        private TcpClient client = null;

        /// <summary>
        /// The peer.
        /// </summary>
        private Peer peer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.AtomicMessageSender"/> class.
        /// </summary>
        /// <param name="peer">Peer that messages will be sent to.</param>
        public AtomicMessageSender(Peer peer)
        {
            this.peer = peer;
            Logger.Info("==============AtomicMessageSender: {0}===============", peer.Port);
        }

        /// <summary>
        /// Occurs when completed.
        /// </summary>
        /// <param name="err">The error.</param>
        public delegate void OnCompleteHandler(Exception err);

        /// <summary>
        /// Occurs when completed.
        /// </summary>
        public event OnCompleteHandler OnComplete;

        /// <summary>
        /// Sends binary data to the specified peer.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public void SendBytes(byte[] bytes)
        {
            Thread thread = new Thread(this.SendBytes);
            thread.Start(bytes);
        }

        /// <summary>
        /// Implementation of sending data.
        /// </summary>
        /// <param name="obj">The bytes.</param>
        private void SendBytes(object obj)
        {
            try
            {
                var bytes = (byte[])obj;
                this.client = new TcpClient();
                this.client.Connect(this.peer.IP, this.peer.Port);

                Stream stmeam = this.client.GetStream();
                stmeam.Write(bytes, 0, bytes.Length);
                stmeam.Flush();

                this.client.Close();

                if (this.OnComplete != null)
                {
                    this.OnComplete(null);
                }
            }
            catch (Exception err)
            {
                if (this.OnComplete != null)
                {
                    this.OnComplete(err);
                }
            }
        }
    }
}
