using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Distribox.CommonLib
{
    public class Peer
    {
        private IPAddress _address;
        private int _port;
		// TODO add a hash member

        public Peer() { }

		public Peer(IPAddress address, int port)
		{
			this._address = address;
			this._port = port;
		}
		
		public Peer(string address, int port)
		{
			this._address = IPAddress.Parse(address);
			this._port = port;
		}

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

        public override int GetHashCode()
        {
            return String.Format("{0}:{1}", IP, Port).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var peer = (Peer)obj;
            return this.IP == peer.IP && this.Port == peer.Port;
        }
    }
    
}
