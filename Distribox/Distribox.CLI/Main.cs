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
        static AntiEntropyProtocol protocol;
		static int port;
		static string root;

        public class API
        {
            public void invite(int port)
            {
                protocol.InvitePeer(new Peer(IPAddress.Parse("127.0.0.1"), port));
            }
        }
		
		public static void Main(string[] args)
        {
			// TODO use a config file to store port and root
			Console.WriteLine("What is my port?");
			port = int.Parse(Console.ReadLine());
			
			Console.WriteLine("What is root?");
			root = Console.ReadLine().Replace("\\", "/") + "/";
			Initialize();
			StartPeer();
		}
		
		private static void StartPeer()
		{
			string peerListName = root + ".Distribox/PeerList.json";
			
			// Initialize anti entropy protocol
			var vs = new VersionControl(root);
			protocol = new AntiEntropyProtocol(port, peerListName, vs);
			
			// Initialize file watcher
			FileWatcher watcher = new FileWatcher(root);
			watcher.Created += x => { lock (vs) vs.Created(x); };
			watcher.Changed += x => { lock (vs) vs.Changed(x); };
			watcher.Deleted += x => { lock (vs) vs.Deleted(x); };
			watcher.Renamed += x => { lock (vs) vs.Renamed(x); };
			watcher.Idle += vs.Flush;


            // Create a virtual machine to repl
            RubyEngine vm = new RubyEngine();
            vm["api"] = new API();
            vm.Repl();
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
                File.WriteAllText(root + ".Distribox/PeerList.json", String.Format("{{ PeerFileName:\"{0}\" }}", root + ".Distribox/PeerList.json"));
			}
		}
	}
}
