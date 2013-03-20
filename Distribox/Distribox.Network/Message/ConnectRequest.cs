using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribox.Network
{
    class ConnectRequest : ProtocolMessage
    {
        public ConnectRequest(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.ConnectRequest;
        }
    }
}
