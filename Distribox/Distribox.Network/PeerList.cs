namespace Distribox.Network
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Distribox.CommonLib;

    /// <summary>
    /// List of peers that ever existed in P2P network. 
    /// </summary>
    internal class PeerList
    {
        private Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.PeerList"/> class.
        /// </summary>
        /// <param name="peerFileName">Peerlist file name.</param>
        public PeerList(string peerFileName)
        {
            this.PeerFileName = peerFileName;
            this.Peers = new HashSet<Peer>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.PeerList"/> class.
        /// </summary>
        public PeerList()
        {
            this.Peers = new HashSet<Peer>();
        }

        /// <summary>
        /// Gets or sets the peers.
        /// </summary>
        /// <value>The peers.</value>
        public HashSet<Peer> Peers { get; set; }

        /// <summary>
        /// Gets or sets the name of the peer file.
        /// </summary>
        /// <value>The name of the peer file.</value>
        public string PeerFileName { get; set; }

        /// <summary>
        /// Gets a peer list.
        /// </summary>
        /// <returns>The peer list.</returns>
        /// <param name="peerFileName">Peer file name.</param>
        public static PeerList GetPeerList(string peerFileName)
        {
            if (File.Exists(peerFileName))
            {
                return CommonHelper.ReadObject<PeerList>(peerFileName);
            }
            else
            {
                return new PeerList(peerFileName);
            }
        }

        /// <summary>
        /// Adds the peer and flush to disk.
        /// </summary>
        /// <param name="peer">Peer.</param>
        public void AddPeer(Peer peer)
        {
            this.AddPeerVolatile(peer);
            this.FlushToDisk();
        }

        /// <summary>
        /// Selects a random peer.
        /// </summary>
        /// <returns>The randomly selected peer.</returns>
        public Peer SelectRandomPeer()
        {
            // TODO efficiency improve required
            ArrayList peers_ram = new ArrayList();
            foreach (Peer peer in this.Peers)
            {
                peers_ram.Add(peer);
            }

            if (peers_ram.Count == 0)
            {
                return null;
            }

            return (Peer)peers_ram[this.random.Next(peers_ram.Count)];
        }

        /// <summary>
        /// Merges the with another peer list.
        /// </summary>
        /// <param name="list">List.</param>
        public void MergeWith(PeerList list)
        {
            foreach (Peer peer in list.Peers)
            {
                if (!this.Peers.Contains(peer))
                {
                    this.AddPeerVolatile(peer);
                }
            }

            this.FlushToDisk();
        }

        /// <summary>
        /// Flushes peerlist file to disk.
        /// </summary>
        private void FlushToDisk()
        {
            this.WriteObject(this.PeerFileName);
        }

        /// <summary>
        /// Adds the peer to memory.
        /// </summary>
        /// <param name="peer">Peer.</param>
        private void AddPeerVolatile(Peer peer)
        {
            if (!this.Peers.Contains(peer))
            {
                this.Peers.Add(peer);
                Logger.Info("New peer: {0}!", peer.IP);
            }
        }
    }
}
