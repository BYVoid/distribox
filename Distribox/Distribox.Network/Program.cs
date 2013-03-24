using System;
using System.Collections.Generic;
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
            StartPeer();
        }

        private static void StartPeer()
        {
            // TODO use a config file to store port and root
            Console.WriteLine("What is my port?");
            int port = int.Parse(Console.ReadLine());

            Console.WriteLine("What is root?");
            string root = Console.ReadLine() + "/";
            string peerListName = root + ".Distribox/PeerList.json";

			// Initialize anti entropy protocol
			var vs = new VersionControl(root);
            AntiEntropyProtocol protocol = new AntiEntropyProtocol(port, peerListName, vs);

            // Initialize file watcher
            FileWatcher watcher = new FileWatcher(root);
            watcher.Created += x => { lock (vs) vs.Created(x); };
            watcher.Changed += x => { lock (vs) vs.Changed(x); };
            watcher.Deleted += x => { lock (vs) vs.Deleted(x); };
            watcher.Renamed += x => { lock (vs) vs.Renamed(x); };
            watcher.Idle += vs.Flush;

            // Create a console for user to invite peer
            Console.WriteLine("Whom should I invite?");
            int i_port = int.Parse(Console.ReadLine());

            Console.WriteLine("Sending invitation...");
            protocol.InvitePeer(new Peer(IPAddress.Parse("127.0.0.1"), i_port));
        }
    }
}
