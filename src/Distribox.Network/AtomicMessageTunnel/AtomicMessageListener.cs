//-----------------------------------------------------------------------
// <copyright file="AtomicMessageListener.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// Listener of messages in P2P network.
    /// </summary>
    internal class AtomicMessageListener
    {
        /// <summary>
        /// The BUFFERSIZ.
        /// </summary>
        private const int BUFFERSIZE = 1000;

        /// <summary>
        /// The listener.
        /// </summary>
        private TcpListener listener = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.AtomicMessageListener"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public AtomicMessageListener(int port)
        {
            this.listener = new TcpListener(IPAddress.Any, port);

            Thread thread = new Thread(this.BackgroundListener);
            thread.Start();
        }

        /// <summary>
        /// Occurs when received data.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="peerFrom">The peer from.</param>
        public delegate void OnReceiveHandler(byte[] data, Peer peerFrom);

        /// <summary>
        /// Occurs when received data.
        /// </summary>
        public event OnReceiveHandler OnReceive;

        /// <summary>
        /// Waits for connection in a thread
        /// </summary>
        private void BackgroundListener()
        {
            this.listener.Start();
            while (true)
            {
                Socket client = this.listener.AcceptSocket();
                Thread thread = new Thread(this.ReceivePackages);
                thread.Start(client);
            }
        }

        /// <summary>
        /// Receive packages.
        /// </summary>
        /// <param name="obj">The client.</param>
        private void ReceivePackages(object obj)
        {
            Socket client = (Socket)obj;
            IPEndPoint ipendpoint = (IPEndPoint)client.RemoteEndPoint;
            Peer peerFrom = new Peer(ipendpoint.Address, ipendpoint.Port);
            List<byte[]> packages = new List<byte[]>();
            int total = 0;
            while (true)
            {
                byte[] buffer = new byte[BUFFERSIZE];
                int bytesReceived = client.Receive(buffer);
                if (bytesReceived == 0)
                {
                    break;
                }

                byte[] exact = new byte[bytesReceived];
                for (int i = 0; i < bytesReceived; i++)
                {
                    exact[i] = buffer[i];
                }

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
            if (this.OnReceive != null)
            {
                this.OnReceive(data, peerFrom);
            }
        }
    }
}
