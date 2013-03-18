using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;
using Distribox.Network;

namespace Distribox.Network
{
    class Program
    {
        static void Main(string[] args)
        {
            client();
        }

        static void server()
        {
            var listener = new AtomicMessageListener(5000);
            listener.OnReceive += listener_OnReceive;
        }

        static void client()
        {
            var sender = new AtomicMessageSender("127.0.0.1", 5000);
            sender.SendBytes(CommonHelper.StringToByte("Hello World!"));
        }

        static void listener_OnReceive(byte[] data, string address)
        {
            Console.WriteLine(address);
            Console.WriteLine("\t{0}", CommonHelper.ByteToString(data));
        }
    }
}
