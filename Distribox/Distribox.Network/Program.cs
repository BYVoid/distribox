using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Distribox.CommonLib;
using Distribox.Network;
using Distribox.Client.Module;
using System.Threading;

namespace Distribox.Network
{
    class Program
    {
        static void Main(string[] args)
        {
            server();
            /*Console.WriteLine("server/client?");
            string s = Console.ReadLine();

            if (s == "client")
                client();
            else if (s == "server")
                server();
            else
            {
                Console.WriteLine("What damn is this? {0}", s);
            }*/
        }

        private static void client()
        {
            Console.WriteLine("I am a client...");

            Console.WriteLine("What is my port?");
            int port = int.Parse(Console.ReadLine());

            Console.WriteLine("What's the name of my PeerList?");
            string peerListName = Console.ReadLine();
            AntiEntropyProtocol protocol = new AntiEntropyProtocol(port, peerListName);
        }
        
        private static void server()
        {
            Console.WriteLine("What is my port?");
            int port = int.Parse(Console.ReadLine());

            Console.WriteLine("What's the name of my PeerList?");
            string peerListName = Console.ReadLine();

            Console.WriteLine("What is root?");
            String root = "E:\\Distribox" + Console.ReadLine() + "\\";

            AntiEntropyProtocol protocol = new AntiEntropyProtocol(port, peerListName);

            var vs = new VersionControl(root);
            protocol.Versions = vs.version_list;

            FileWatcher watcher = new FileWatcher(root);
            watcher.Created += x => { lock (vs) vs.Created(x); };
            watcher.Changed += x => { lock (vs) vs.Changed(x); };
            watcher.Deleted += x => { lock (vs) vs.Deleted(x); };
            watcher.Renamed += x => { lock (vs) vs.Renamed(x); };
            watcher.Idle += vs.Flush;

            Thread thread = new Thread(() =>
                {
                    Console.WriteLine("Whom should I invite?");
                    int i_port = int.Parse(Console.ReadLine());

                    Console.WriteLine("Sending invitation...");
                    protocol.InvitePeer(new Peer(IPAddress.Parse("127.0.0.1"), i_port));
                });
            thread.Start();

            Console.WriteLine("Here!");
            watcher.WaitForEvent();
        }

    }
}
