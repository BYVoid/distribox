using System;
using System.IO;
using System.Net;
using Distribox.CommonLib;
using Distribox.FileSystem;
using Distribox.Network;

namespace Distribox.CLI
{
    /// <summary>
    /// Root of the CLI program.
    /// </summary>
    class Program
    {
        static AntiEntropyProtocol protocol;
  
        /// <summary>
        /// Used for the ruby engine.
        /// </summary>
        public class API
        {
            /// <summary>
            /// Invite the specified peer at <paramref name="port"/>.
            /// </summary>
            /// <param name='port'>
            /// Port of the peer to be invited.
            /// </param>
            /// <remarks>
            /// Currently, we are only communicating on a single machine. So no ip is specified.s
            /// </remarks>
            public void invite(int port)
            {
                protocol.InvitePeer(new Peer(IPAddress.Parse("127.0.0.1"), port));
            }
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            // Get config
            int port = Config.GetConfig().ListenPort;
            string root = Config.GetConfig().RootFolder;
            
            // Initialize folder
            CommonHelper.InitializeFolder(root);
            
            // Start peer service
            StartPeer(port, root);
   
            // Start CLI
            RubyEngine engine = new RubyEngine();
            engine["api"] = new API();
            engine.Repl();
        }
        
        /// <summary>
        /// Starts the peer service.
        /// </summary>
        /// <param name='port'>
        /// Listening port of the peer.
        /// </param>
        /// <param name='root'>
        /// Root folder to be watched.
        /// </param>
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
