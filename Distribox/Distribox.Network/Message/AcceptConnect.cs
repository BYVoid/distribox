using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribox.Network
{
    class AcceptConnect : ProtocolMessage
    {
        public AcceptConnect(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.AcceptConnect;
        }
    }
}
