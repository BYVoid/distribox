using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribox.Network
{
    class Invitation : ProtocolMessage
    {
        public Invitation(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.Invitation;
        }
    }
}
