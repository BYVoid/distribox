using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Distribox.Network.Communication
{
    class ConnectionEstablishedEventArgs
    {
        public AtomicMessageTunnel tunnel;

        /// <summary>
        /// If the returned AtomicMessageTunnel is created by ConnectWith, the value will be true.
        /// If the returned AtomicMessageTunnel is created by Listen, the value will be false.
        /// </summary>
        public bool isActive;

        public ConnectionEstablishedEventArgs(AtomicMessageTunnel tunnel, bool isActive)
        {
            this.tunnel = tunnel;
            this.isActive = isActive;
        }
    }

    class AtomicMessageTunnelFactory 
    {
        private IPEndPoint _myEndPoint;
        private TcpListener listener = null;

        /// <summary>
        /// Return created instance of AtomicMessageTunnel. The instance is created either by 
        /// ConnectWith (connect to other peer actively) or by Listen (wait for other peer's connection passively).
        /// </summary>
        /// <param name="tunnel"></param>
        /// <param name="args">Other information of the returned instance. Including a value indicating if the instance is created by ConnectWith or Listen</param>        
        public delegate void ConnectionEstablishedHandler(ConnectionEstablishedEventArgs args);
        public event ConnectionEstablishedHandler ConnectionEstblished;        

        private void OnConnectionEstablished(ConnectionEstablishedEventArgs args)
        {
            if (ConnectionEstblished != null)
                ConnectionEstblished(args);
        }

        public void FinishIncomingConnection(IAsyncResult ar) 
        {
            TcpListener listener = (TcpListener) ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            OnConnectionEstablished(new ConnectionEstablishedEventArgs(
                /*tunnel=*/ new AtomicMessageTunnel(client), 
                /*isActive=*/ false)
            );

            StartListen();
        }

        public void FinishOutcomingConnection(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            client.EndConnect(ar);
            OnConnectionEstablished(new ConnectionEstablishedEventArgs(
                /*tunnel=*/ new AtomicMessageTunnel(client), 
                /*isActive=*/true)
            );
        }

        public AtomicMessageTunnelFactory(IPEndPoint myEndPoint)
        {
            _myEndPoint = myEndPoint;
        }        

        public void StartConnectWith(IPEndPoint serverEndPoint)
        {
            TcpClient client = new TcpClient(_myEndPoint);

            client.BeginConnect(serverEndPoint.Address, serverEndPoint.Port, new AsyncCallback(FinishOutcomingConnection), client);
        }

        public void StartListen()
        {
            if (listener == null)
            {
                listener = new TcpListener(_myEndPoint);
                listener.Start();
            }

            listener.BeginAcceptTcpClient(new AsyncCallback(FinishIncomingConnection), listener);
        }
    }
}
