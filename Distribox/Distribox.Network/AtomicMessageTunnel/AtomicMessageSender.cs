using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Distribox.CommonLib;

namespace Distribox.Network
{
    /// <summary>
    /// Sender of messages to a specified peer.
    /// </summary>
    class AtomicMessageSender
    {
        /// <summary>
        /// Occurs when completed.
        /// </summary>
        public delegate void OnCompleteHandler(Exception err);

        /// <summary>
        /// Occurs when completed.
        /// </summary>
        public event OnCompleteHandler OnComplete;

        private TcpClient _client = null;
        private Peer _peer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.AtomicMessageSender"/> class.
        /// </summary>
        /// <param name="peer">Peer that messages will be sent to.</param>
        public AtomicMessageSender(Peer peer)
        {
            this._peer = peer;
            Logger.Info("==============AtomicMessageSender: {0}===============", _peer.Port);
        }

        /// <summary>
        /// Sends binary data to the specified peer.
        /// </summary>
        /// <param name="bytes">Bytes.</param>
        public void SendBytes(byte[] bytes)
        {
            Thread thread = new Thread(SendBytes);
            thread.Start(bytes);
        }

        /// <summary>
        /// Implementation of sending data.
        /// </summary>
        /// <param name="_bytes">_bytes.</param>
        private void SendBytes(object _bytes)
        {
            try
            {
                var bytes = (byte[])_bytes;
                _client = new TcpClient();
                _client.Connect(_peer.IP, _peer.Port);

                Stream stmeam = _client.GetStream();
                stmeam.Write(bytes, 0, bytes.Length);
                stmeam.Flush();

                _client.Close();

                if (OnComplete != null)
                {
                    OnComplete(null);
                }
            }
            catch (Exception err)
            {
                if (OnComplete != null)
                {
                    OnComplete(err);
                }
            }
        }
    }
}
