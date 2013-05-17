//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.CLI
{
    using System;
    using System.IO;
    using System.Net;
    using Distribox.CommonLib;
    using Distribox.FileSystem;
    using Distribox.Network;
    using System.Threading;

    /// <summary>
    /// Root of the CLI program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The protocol.
        /// </summary>
        private static AntiEntropyProtocol protocol;

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            // Get config
            int port = Config.ListenPort;

            // Initialize folder
            CommonHelper.InitializeFolder();
            
            // Start peer service
            StartPeer(port);
   
            // Start CLI
            RubyEngine engine = new RubyEngine();
            engine.SetVariable("api", new API());
            engine.Repl();
        }

        /// <summary>
        /// Starts the peer service.
        /// </summary>
        /// <param name='port'>
        /// Listening port of the peer.
        /// </param>
        private static void StartPeer(int port)
        {
            string peerListName = Config.PeerListFilePath;
            
            // Initialize anti entropy protocol
            var vs = new VersionControl();
            protocol = new AntiEntropyProtocol(port, peerListName, vs);
            
            // Initialize file watcher
            FileWatcher watcher = new FileWatcher();
            watcher.Created += vs.Created;
            watcher.Changed += vs.Changed;
            watcher.Deleted += vs.Deleted;
            watcher.Renamed += vs.Renamed;
            watcher.Idle += vs.Flush;
        }
        
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
            public void Invite(string ip, int port)
            {
                protocol.InvitePeer(new Peer(IPAddress.Parse(ip), port));
            }
        }
    }
}
