using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Distribox.Network.Communication.Test
{

    class TestCommunication
    {
        private static void PrintConnection(ConnectionEstablishedEventArgs args)
        {
            Console.WriteLine("Connection Established! isActive={0}", args.isActive);
            Console.WriteLine(args.tunnel);
        }
        
        private static void TestAsClient()
        {            
            Console.WriteLine("Test As Client......");
            Console.WriteLine("Client port = ?");
            int clientPort = int.Parse(Console.ReadLine());

            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), clientPort++);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
            AtomicMessageTunnelFactory factory = new AtomicMessageTunnelFactory(clientEndPoint);
            factory.ConnectionEstblished += PrintConnection;
            factory.StartConnectWith(serverEndPoint);

            while (true) ;
        }

        private static void TestAsServer()
        {
            Console.WriteLine("Test As Server......");

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
            AtomicMessageTunnelFactory factory = new AtomicMessageTunnelFactory(serverEndPoint);
            factory.ConnectionEstblished += PrintConnection;
            factory.StartListen();

            while (true) ;
        }

        public static void Test()
        {
            Console.WriteLine("server / client?");
            string command = Console.ReadLine();

            if (command == "server")
            {
                TestAsServer();
            }
            else if (command == "client")
            {
                TestAsClient();
            }
            else
            {
                Console.WriteLine("Invalid operation...");
            }
        }
    }
}
