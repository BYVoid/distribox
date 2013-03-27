using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Distribox.CommonLib;

namespace Distribox.Network
{
    class PeerList
    {
		public HashSet<Peer> Peers { get; set;}
		public string PeerFileName { get; set;}
        private Random _random = new Random();

		/// <summary>
		/// Flushes peerlist file to disk.
		/// </summary>
        private void FlushToDisk()
        {
			this.WriteObject(PeerFileName);
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Distribox.Network.PeerList"/> class.
		/// </summary>
		/// <param name="peerFileName">Peerlist file name.</param>
        public PeerList(string peerFileName)
        {
            PeerFileName = peerFileName;
            Peers = new HashSet<Peer>();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Distribox.Network.PeerList"/> class.
		/// </summary>
		public PeerList()
        {
            Peers = new HashSet<Peer>();
		}

		/// <summary>
		/// Gets a peer list.
		/// </summary>
		/// <returns>The peer list.</returns>
		/// <param name="peerFileName">Peer file name.</param>
        static public PeerList GetPeerList(string peerFileName)
        {
            if (File.Exists(peerFileName))
                return CommonHelper.ReadObject<PeerList>(peerFileName);
            else
                return new PeerList(peerFileName);
        }

		/// <summary>
		/// Adds the peer and flush to disk.
		/// </summary>
		/// <param name="peer">Peer.</param>
        public void AddPeer(Peer peer)
        {
            AddPeerVolatile(peer);
            FlushToDisk();
        }

		/// <summary>
		/// Adds the peer to memory.
		/// </summary>
		/// <param name="peer">Peer.</param>
        private void AddPeerVolatile(Peer peer)
        {
            if (!(Peers.Contains(peer)))
            {
                Peers.Add(peer);
                Logger.Info("New peer: {0}!", peer.IP);
            }
        }

		/// <summary>
		/// Selects a random peer.
		/// </summary>
		/// <returns>The randomly selected peer.</returns>
        public Peer SelectRandomPeer()
        {
			// TODO efficiency improve required
            ArrayList peers_ram = new ArrayList();
            foreach (Peer peer in Peers)
			{
				peers_ram.Add(peer);
			}

            if (peers_ram.Count == 0)
                return null;
            return (Peer)peers_ram[_random.Next(peers_ram.Count)];
        }

		/// <summary>
		/// Merges the with another peer list.
		/// </summary>
		/// <param name="list">List.</param>
        public void MergeWith(PeerList list)
        {
            foreach (Peer peer in list.Peers)
                if (!Peers.Contains(peer))
                    AddPeerVolatile(peer);

            FlushToDisk();
        }
    }
}
