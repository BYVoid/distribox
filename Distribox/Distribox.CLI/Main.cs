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
			root = Console.ReadLine() + Properties.PathSep;
			Initialize();
			StartPeer();
		}
		
		private static void StartPeer()
		{
			string peerListName = root + Properties.PeerListFilePath;
			
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
			if (!Directory.Exists(root + Properties.MetaFolder))
			{
				Directory.CreateDirectory(root + Properties.MetaFolder);
			}
			if (!Directory.Exists(root + Properties.MetaFolderTmp))
			{
				Directory.CreateDirectory(root + Properties.MetaFolderTmp);
			}
			if (!Directory.Exists(root + Properties.MetaFolderData))
			{
				Directory.CreateDirectory(root + Properties.MetaFolderData);
			}
			if (!File.Exists(root + Properties.VersionListFilePath))
			{
				File.WriteAllText(root + Properties.VersionListFilePath, "[]");
			}
			if (!File.Exists(root + Properties.PeerListFilePath))
			{
				File.WriteAllText(root + Properties.PeerListFilePath, "{}");
			}
		}
	}
}
