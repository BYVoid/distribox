using System;
using System.IO;
using System.Net;
using Distribox.CommonLib;
using Distribox.FileSystem;
using Distribox.Network;

namespace Distribox.CLI
{
	class Program
	{
		public static void Main(string[] args)
		{
            int port = Config.GetConfig().ListenPort;
            string root = Config.GetConfig().RootFolder;

			CommonHelper.InitializeFolder(root);
			StartPeer(port, root);
		}
		
		private static void StartPeer(int port, string root)
		{
			string peerListName = root + Properties.PeerListFilePath;
			
			// Initialize anti entropy protocol
			var vs = new VersionControl(root);
			AntiEntropyProtocol protocol = new AntiEntropyProtocol(port, peerListName, vs);
			
			// Initialize file watcher
			FileWatcher watcher = new FileWatcher();
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
