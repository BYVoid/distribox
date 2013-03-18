using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Distribox.Network.Communication
{
    class ConnectionEstablishedEventArgs
    {
        /// <summary>
        /// If the returned AtomicMessageTunnel is created by ConnectWith, the value will be true.
        /// If the returned AtomicMessageTunnel is created by Listen, the value will be false.
        /// </summary>
        bool isActive;
    }    

    interface IAtomicMessageTunnelFactory
    {           

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myEndPoint">The IP and port of this peer. The value will be used for listening for connection.</param>
        //public IAtomicMessageTunnelFactory(IPEndPoint myEndPoint);

        /// <summary>
        /// Connect to another peer.
        /// </summary>
        /// <param name="serverEndPoint">The IP and port of the peer to be connected.</param>
        /public void ConnectWith(IPEndPoint serverEndPoint);

        /// <summary>
        /// Listen on myEndPoint, which is passed when construction
        /// </summary>
        public void Listen();
    }
}
