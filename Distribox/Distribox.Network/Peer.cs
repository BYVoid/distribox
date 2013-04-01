using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Distribox.CommonLib;

namespace Distribox.Network
{
    /// <summary>
    /// Peer to connect with.
    /// </summary>
    public class Peer
    {
        private IPAddress _address;
        private int _port;

        // TODO add a hash member

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.Peer"/> class using default IP and port.
        /// </summary>
        public Peer()
        {
            this._address = IPAddress.Any;
            this._port = Properties.DefaultListenPort;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.Peer"/> class.
        /// </summary>
        /// <param name="address">IP Address.</param>
        /// <param name="port">Port.</param>
        public Peer(IPAddress address, int port)
        {
            this._address = address;
            this._port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.Peer"/> class.
        /// </summary>
        /// <param name="address">IP Address.</param>
        /// <param name="port">Port number.</param>
        public Peer(string address, int port)
        {
            this._address = IPAddress.Parse(address);
            this._port = port;
        }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        /// <value>The I.</value>
        public String IP
        {
            get
            {
                return _address.ToString();
            }

            set
            {
                _address = IPAddress.Parse(value);
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
            }
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Distribox.Network.Peer"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return String.Format("{0}:{1}", IP, Port).GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Distribox.Network.Peer"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Distribox.Network.Peer"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Distribox.Network.Peer"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var peer = (Peer)obj;
            return this.IP == peer.IP && this.Port == peer.Port;
        }
    }
}
