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

        public class API
        {
            public void invite(int port)
            {
                protocol.InvitePeer(new Peer(IPAddress.Parse("127.0.0.1"), port));
            }
        }
		
		public static void Main(string[] args)
        {
            int port = Config.GetConfig().ListenPort;
            string root = Config.GetConfig().RootFolder;
			CommonHelper.InitializeFolder(root);
			StartPeer(port, root);

            RubyEngine engine = new RubyEngine();
            engine["api"] = new API();
            engine.Repl();
		}
		
		private static void StartPeer(int port, string root)
		{
			string peerListName = root + Properties.PeerListFilePath;
			
			// Initialize anti entropy protocol
			var vs = new VersionControl(root);
			protocol = new AntiEntropyProtocol(port, peerListName, vs);
			
			// Initialize file watcher
			FileWatcher watcher = new FileWatcher();
			watcher.Created += vs.Created;
			watcher.Changed += vs.Changed;
			watcher.Deleted += vs.Deleted;
			watcher.Renamed += vs.Renamed;
			watcher.Idle += vs.Flush;


            // Create a virtual machine to repl
            RubyEngine vm = new RubyEngine();
            vm["api"] = new API();
            vm.Repl();
		}
	}
}
