using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribox.Network
{
    class ProtocolMessage
    {
        // FIXME: I set all of these public for JSON serialize
        public enum MessageType
        {
            Invitation,
            AcceptInvitation,
            ConnectRequest,
            AcceptConnect,
            PeerListMessage
        }
        public int MyListenPort;
        public MessageType _type;

        public ProtocolMessage(int myListenPort)
        {
            MyListenPort = myListenPort;
        }

        public ProtocolMessage ParseToDerivedClass(byte[] data)
        {
            switch (_type)
            {
                case MessageType.Invitation:
                    return CommonLib.CommonHelper.Read<Invitation>(data);
                case MessageType.AcceptInvitation:
                    return CommonLib.CommonHelper.Read<AcceptInvitation>(data);
                case MessageType.ConnectRequest:
                    return CommonLib.CommonHelper.Read<ConnectRequest>(data);
                case MessageType.AcceptConnect:
                    return CommonLib.CommonHelper.Read<AcceptConnect>(data);
                case MessageType.PeerListMessage:
                    return CommonLib.CommonHelper.Read<PeerListMessage>(data);
            }
            throw new Exception("ToDerivedClass: What class is this? Maybe you forgot to add an enum / case statement?");            
        }
    }
}
