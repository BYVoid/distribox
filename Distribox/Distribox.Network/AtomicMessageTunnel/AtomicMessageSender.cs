using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Distribox.Network
{
    public class AtomicMessageSender
    {
		public delegate void OnCompleteHandler();
		public event OnCompleteHandler OnComplete;
		public event OnCompleteHandler OnError; // TODO implement on error

        private byte[] _buffer = new byte[10000];
        private TcpClient _client = null;
        private String ip = null;
        private int port = 0;

        public AtomicMessageSender(String ip, int port)
        {
			// logger
            Console.WriteLine("==============AtomicMessageSender: {0}===============", port);
            this.ip = ip;
            this.port = port;
        }

        public void SendBytes(byte[] bytes)
        {
            Thread thread = new Thread(SendInfo);
            thread.Start(bytes);
        }

        private void SendInfo(object _bytes)
        {
            var bytes = (byte[])_bytes;
            _client = new TcpClient();
            _client.Connect(ip, port);

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
