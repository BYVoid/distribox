using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribox.Network
{
    class AcceptInvitation : ProtocolMessage
    {
        public AcceptInvitation(int myListenPort)
            : base(myListenPort)
        {
            _type = MessageType.AcceptInvitation;
        }
    }
}
