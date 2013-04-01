using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Distribox.CommonLib;

namespace Distribox.Network
{
    /// <summary>
    /// Listener of messages in P2P network.
    /// </summary>
    public class AtomicMessageListener
    {
        /// <summary>
        /// Occurs when received data.
        /// </summary>
        public delegate void OnReceiveHandler(byte[] data, Peer peerFrom);

        /// <summary>
        /// Occurs when received data.
        /// </summary>
        public event OnReceiveHandler OnReceive;

        private TcpListener _listener = null;
        private const int BUFFER_SIZE = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.AtomicMessageListener"/> class.
        /// </summary>
        /// <param name="port">Port.</param>
        public AtomicMessageListener(int port)
        {
            Console.WriteLine("==============AtomicMessageListener: {0}===============", port);
            _listener = new TcpListener(IPAddress.Any, port);

            Thread thread = new Thread(BackgroundListener);
            thread.Start();
        }

        /// <summary>
        /// Waits for connection in a thread
        /// </summary>
        private void BackgroundListener()
        {
            _listener.Start();
            while (true)
            {
                Socket client = _listener.AcceptSocket();
                Thread thread = new Thread(ReceivePackages);
                thread.Start(client);
            }
        }

        /// <summary>
        /// Reveive packages.
        /// </summary>
        /// <param name="_client">_client.</param>
        private void ReceivePackages(Object _client)
        {
            Socket client = (Socket)_client;
            IPEndPoint ipendpoint = ((IPEndPoint)client.RemoteEndPoint);
            Peer peerFrom = new Peer(ipendpoint.Address, ipendpoint.Port);
            List<byte[]> packages = new List<byte[]>();
            int total = 0;
            while (true)
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesReceived = client.Receive(buffer);
                if (bytesReceived == 0)
                    break;
                byte[] exact = new byte[bytesReceived];
                for (int i = 0; i < bytesReceived; i++)
                    exact[i] = buffer[i];
                packages.Add(exact);
                total = total + bytesReceived;
            }
            client.Close();

            // Merge all packages into a byte array
            byte[] data = new byte[total];
            int ct = 0;
            foreach (var array in packages)
            {
                foreach (var x in array)
                {
                    data[ct++] = x;
                }
            }

            // Callback
            if (OnReceive != null)
            {
                OnReceive(data, peerFrom);
            }
        }
    }
}
