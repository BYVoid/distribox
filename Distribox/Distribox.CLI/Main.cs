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
		static int port;
		static string root;
		
		public static void Main(string[] args)
		{
			// TODO use a config file to store port and root
			Console.WriteLine("What is my port?");
			port = int.Parse(Console.ReadLine());
			
			Console.WriteLine("What is root?");
			root = Console.ReadLine() + "/";
			Initialize();
			StartPeer();
		}
		
		private static void StartPeer()
		{
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
		
		/// <summary>
		/// Initialize the folders and version list.
		/// </summary>
		private static void Initialize()
		{
			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}
			if (!Directory.Exists(root + ".Distribox"))
			{
				Directory.CreateDirectory(root + ".Distribox");
			}
			if (!Directory.Exists(root + ".Distribox/tmp"))
			{
				Directory.CreateDirectory(root + ".Distribox/tmp");
			}
			if (!Directory.Exists(root + ".Distribox/data"))
			{
				Directory.CreateDirectory(root + ".Distribox/data");
			}
			if (!File.Exists(root + ".Distribox/VersionList.txt"))
			{
				File.WriteAllText(root + ".Distribox/VersionList.txt", "[]");
			}
			if (!File.Exists(root + ".Distribox/PeerList.json"))
			{
				File.WriteAllText(root + ".Distribox/PeerList.json", "{}");
			}
		}
	}
}
