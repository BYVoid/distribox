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
    class AtomicMessageSender
    {
		public delegate void OnCompleteHandler();
		public event OnCompleteHandler OnComplete;
		public event OnCompleteHandler OnError; // TODO implement on error

        private TcpClient _client = null;
		private Peer _peer;

        public AtomicMessageSender(Peer peer)
        {
			// TODO logger
            Console.WriteLine("==============AtomicMessageSender: {0}===============", _peer.Port);
			this._peer = peer;
        }

        public void SendBytes(byte[] bytes)
        {
            Thread thread = new Thread(SendBytes);
            thread.Start(bytes);
        }

        private void SendBytes(object _bytes)
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
                OnComplete();
            }
        }

    }
}
