using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Distribox.Network
{
    public class AtomicMessageListener
    {
        public delegate void OnReceiveHandler(byte[] data, String address);
        public event OnReceiveHandler OnReceive;

        private TcpListener _listener = null;
        private int _port;

        public AtomicMessageListener(int port)
        {
            Console.WriteLine("==============AtomicMessageListener: {0}===============", port);
            _listener = new TcpListener(IPAddress.Any, port);
            _port = port;

            Thread listener = new Thread(BackgroundListener);
            listener.Start();
        }

        private void BackgroundListener()
        {
            _listener.Start();
            while (true)
            {
                Socket client = _listener.AcceptSocket();
                Thread thread = new Thread(StartReceive);
                StartReceive(client);
            }
        }

        private void StartReceive(Object _client)
        {
            Socket client = (Socket)_client;
            IPEndPoint ipendpoint = ((IPEndPoint)client.RemoteEndPoint);
            String Address = ipendpoint.Address.ToString() + ":" + ipendpoint.Port;
            List<byte[]> packages = new List<byte[]>();
            int total = 0;
            while (true)
            {
                byte[] buffer = new byte[1000];
                int k = client.Receive(buffer);
                if (k == 0) break;
                byte[] exact = new byte[k];
                for (int i = 0; i < k; i++) exact[i] = buffer[i];
                packages.Add(exact);
                total = total + k;
            }
            client.Close();

            byte[] data = new byte[total];
            int ct = 0;
            foreach (var array in packages)
                foreach (var x in array)
                {
                    data[ct++] = x;
                }
            if (OnReceive != null)
            {
                OnReceive(data, Address);
            }
        }

    }
}
