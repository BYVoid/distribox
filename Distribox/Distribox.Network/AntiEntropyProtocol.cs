using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Distribox.CommonLib;
using System.IO;

namespace Distribox.Network
{
	class AntiEntropyProtocol
    {
		// TODO use config file
        private const int CONNECT_PERIOD_MS = 1000;

        private PeerList _peers;
        private AtomicMessageListener _listener;
        private int _listeningPort;

        public VersionList Versions { get; set; }

		/// <summary>
		/// Handle receive message event.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="address">Address.</param>
        private void OnReceiveMessage(byte[] data, string address)
		{
			ParseAndDispatchMessage(data, address);
		}

		/// <summary>
		/// Parses and dispatch message from peer.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="address">Address.</param>
		private void ParseAndDispatchMessage(byte[] data, string address)
		{
			// Parse it, and convert to the right derived class
			ProtocolMessage message = CommonHelper.Read<ProtocolMessage>(data).ParseToDerivedClass(data);            
			
			// TODO use a more specific expression to present message source rather than a string of address
			// Parse IP and Port
			string[] ipAndPort = address.Split(':');
			string ip = ipAndPort[0];
			// ipAndPort[1] is the port of the sender socket, but we need the number of the listener port......
			int port = message.MyListenPort;
			Peer peer = new Peer(IPAddress.Parse(ip), port);
			
			// Process message (visitor design pattern)
			message.Accept(this, peer);
		}

		/// <summary>
		/// Sends a message to peer.
		/// </summary>
		/// <param name="peer">Peer.</param>
		/// <param name="message">Message.</param>
		/// <param name="onCompleteHandler">On complete handler.</param>
        private static void SendMessage(Peer peer, ProtocolMessage message, AtomicMessageSender.OnCompleteHandler onCompleteHandler = null)
        {
            AtomicMessageSender sender = new Network.AtomicMessageSender(peer.IP, peer.Port);
            if (onCompleteHandler != null)
            {
                sender.OnComplete += onCompleteHandler;
            }
            byte[] bMessage = CommonLib.CommonHelper.ShowAsBytes(message);
            //Console.WriteLine(CommonLib.CommonHelper.ByteToString(bMessage)); TODO use logger
            sender.SendBytes(bMessage);
			// TODO on error
        }

		/// <summary>
		/// Process the invitation message.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		public void Process(InvitationRequest message, Peer peer)
        {
            // Send AcceptInvivation back
            SendMessage(peer, new InvitationAck(_listeningPort));
        }

		/// <summary>
		/// Process the accept message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		public void Process(InvitationAck message, Peer peer)
        {
            // Try to sync with the newly accepted peer
            SendMessage(peer, new SyncRequest(_listeningPort));
        }

		/// <summary>
		/// Process the sync message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		public void Process(SyncRequest message, Peer peer)
        {
            // Accept the sync request
			SendMessage(peer, new SyncAck(_listeningPort));

			// Send MetaData
            SendMetaData(peer);
        }

		/// <summary>
		/// Process the sync ack message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		public void Process(SyncAck message, Peer peer)
        {
            SendMetaData(peer);
        }

		/// <summary>
		/// Process the peer list message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		public void Process(PeerListMessage message, Peer peer)
        {
            lock (_peers)
            {
                _peers.AddPeerAndFlush(peer);
                _peers.MergeWith(message.List);
            }
        }

		/// <summary>
		/// Process the version list message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		public void Process(VersionListMessage message, Peer peer)
        {            
            List<FileItem> versionRequest;
            lock (Versions)
            {
                versionRequest = Versions.GetLessThan(message.List);
				// TODO logger
                Console.WriteLine("Received version list from {1}\n{0}", message.List.Show(), peer.Show());
            }            
            SendMessage(peer, new FileRequest(versionRequest, _listeningPort));

            Console.WriteLine("Sent file request\n{0}", versionRequest.Show());
        }

		/// <summary>
		/// Process the file request message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		public void Process(FileRequest message, Peer peer)
        {
			// TODO logger
            Console.WriteLine("Receive file request\n{0}", message._request.Show());

			string filename = null;
            lock (Versions)
            {
                filename = Versions.CreateFileBundle(message._request);
            }

            byte[] data = File.ReadAllBytes(filename);
            SendMessage(peer, new FileDataResponse(data, _listeningPort), () => File.Delete(filename));
        }

		/// <summary>
		/// Process the specified message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		public void Process(FileDataResponse message, Peer peer)
        {
            lock(Versions)
            {
                Versions.AcceptFileBundle(message._data);
            }
        }

		/// <summary>
		/// Sends the meta data.
		/// </summary>
		/// <param name="peer">Peer.</param>
        private void SendMetaData(Peer peer)
        {
            // Send PeerList
			SendMessage(peer, new PeerListMessage(_peers, _listeningPort));

			// Send VersionList
            SendMessage(peer, new VersionListMessage(Versions, _listeningPort));

			// TODO logger
            Console.WriteLine("Send version list to {0}", peer.Show());
            Console.WriteLine(Versions.Show());
        }

		/// <summary>
		/// Connects a random peer to send a connection request.
		/// </summary>
        private void ConnectRandomPeer()
        {
            Peer peer;
			// Lock peers to support thread-safety
            lock (_peers)
            {
                // FIXME Judge if the randomly picked peer is itself using hash
				// caution: dead loop
                do
                {
                    peer = _peers.SelectRandomPeer();
                }
                while (peer.Port == _listeningPort);
            }
            if (peer != null)
			{
                SendMessage(peer, new SyncRequest(_listeningPort));
			}
        }

		/// <summary>
		/// Handle timer event.
		/// </summary>
		/// <param name="source">Source of event.</param>
		/// <param name="e">Event.</param>
        private void OnTimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            ConnectRandomPeer();
        }  

		/// <summary>
		/// Initializes a new instance of the <see cref="Distribox.Network.AntiEntropyProtocol"/> class.
		/// </summary>
		/// <param name="listeningPort">Listening port.</param>
		/// <param name="peerFileName">File name of peer list.</param>
		public AntiEntropyProtocol(int listeningPort, string peerFileName)
        {
            // Initialize peer list
            _peers = PeerList.GetPeerList(peerFileName);
			_listeningPort = listeningPort;

            // Initialize listener
			_listener = new AtomicMessageListener(listeningPort);
            _listener.OnReceive += OnReceiveMessage;

            // Initialize timer to connect other peers periodically
            System.Timers.Timer timer = new System.Timers.Timer(CONNECT_PERIOD_MS);
            timer.Elapsed += this.OnTimerEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

		/// <summary>
		/// Invites a peer into P2P network.
		/// </summary>
		/// <param name="peer">The peer to be invited.</param>
        public void InvitePeer(Peer peer)
        {
            SendMessage(peer, new InvitationRequest(_listeningPort));
        }
    }
}
