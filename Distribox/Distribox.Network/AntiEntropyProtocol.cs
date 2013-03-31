using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections;
using Distribox.CommonLib;
using Distribox.FileSystem;

namespace Distribox.Network
{
	public class AntiEntropyProtocol
    {
        private PeerList _peers;
        private AtomicMessageListener _listener;
        private int _listeningPort;

		private VersionControl _versionControl;

        // Use <AtomicPatch> as its member
        private RequestManager _requestManager;

		/// <summary>
		/// Handle receive message event.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="peerFrom">peer from.</param>
        private void OnReceiveMessage(byte[] data, Peer peerFrom)
		{
			ParseAndDispatchMessage(data, peerFrom);
		}

		/// <summary>
		/// Parses and dispatch message from peer.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="peerFrom">Address.</param>
		private void ParseAndDispatchMessage(byte[] data, Peer peerFrom)
		{
			// Parse it, and convert to the right derived class
			ProtocolMessage message = CommonHelper.Deserialize<ProtocolMessage>(data).ParseToDerivedClass(data);            

			// ipAndPort[1] is the port of the sender socket, but we need the number of the listener port......
			int port = message.MyListenPort;
			Peer peer = new Peer(peerFrom.IP, port);
			
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
            AtomicMessageSender sender = new Network.AtomicMessageSender(peer);
            if (onCompleteHandler != null)
            {
                sender.OnComplete += onCompleteHandler;
            }
            byte[] bMessage = CommonLib.CommonHelper.SerializeAsBytes(message);
            Console.WriteLine(message);
            sender.SendBytes(bMessage);
			// TODO on error
        }

        /// <summary>
        /// This routine will decide if to start a new request. If so, it will get an request from RequestManager and send out the request message
        /// </summary>
        private void TryToRequest()
        {
            // TODO don't request if too many connections
            while (true)
            {
                // Get a request from RequestManager
                var requestTuple = _requestManager.GetRequests();

                // Maybe there are not any requests...
                if (requestTuple == null)
                    return;

                List<AtomicPatch> patches = requestTuple.Item1;
                Peer peer = requestTuple.Item2;

                // Send out the request
                Console.WriteLine(new PatchRequest(patches, _listeningPort).Serialize());
                SendMessage(peer, new PatchRequest(patches, _listeningPort));
            }            
        }

		/// <summary>
		/// Process the invitation message.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		internal void Process(InvitationRequest message, Peer peer)
        {
            // Send AcceptInvivation back
            SendMessage(peer, new InvitationAck(_listeningPort));
        }

		/// <summary>
		/// Process the accept message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		internal void Process(InvitationAck message, Peer peer)
        {
            // Try to sync with the newly accepted peer
            SendMessage(peer, new SyncRequest(_listeningPort));
        }

		/// <summary>
		/// Process the sync message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		internal void Process(SyncRequest message, Peer peer)
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
		internal void Process(SyncAck message, Peer peer)
        {
            SendMetaData(peer);
        }

		/// <summary>
		/// Process the peer list message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		internal void Process(PeerListMessage message, Peer peer)
        {
            lock (_peers)
            {
                _peers.AddPeer(peer);
                _peers.MergeWith(message.List);
            }
        }

		/// <summary>
		/// Process the version list message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		internal void Process(VersionListMessage message, Peer peer)
        {            
            List<AtomicPatch> versionRequest;
            lock (_versionControl.VersionList)
            {
                versionRequest = _versionControl.VersionList.GetLessThan(message.List);
                Logger.Info("Received version list from {1}\n{0}", message.List.Serialize(), peer.Serialize());
            }

            // Don't request them now, add them to a request queue first
            _requestManager.AddRequests(versionRequest, peer);
            TryToRequest();
        }

		/// <summary>
		/// Process the file request message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		internal void Process(PatchRequest message, Peer peer)
        {
            Logger.Info("Receive file request\n{0}", message.Request.Serialize());

			string filename = null;
            lock (_versionControl.VersionList)
            {
                filename = _versionControl.CreateFileBundle(message.Request);
            }

            byte[] data = File.ReadAllBytes(filename);
            SendMessage(peer, new FileDataResponse(data, _listeningPort), (err) => File.Delete(filename));
        }

		/// <summary>
		/// Process the specified message and peer.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="peer">Peer.</param>
		internal void Process(FileDataResponse message, Peer peer)
        {
            List<AtomicPatch> patches;
            lock(_versionControl.VersionList)
            {
                 patches = _versionControl.AcceptFileBundle(message._data);
            }

            _requestManager.FinishRequests(patches);
            TryToRequest();
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
            SendMessage(peer, new VersionListMessage(_versionControl.VersionList, _listeningPort));

			// TODO logger
            Console.WriteLine("Send version list to {0}", peer.Serialize());
            Console.WriteLine(_versionControl.VersionList.Serialize());
        }

		/// <summary>
		/// Connects a random peer to send a connection request.
		/// </summary>
        private void ConnectRandomPeer()
        {
            if (_peers.Peers.Count() == 0) return;
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
		public AntiEntropyProtocol(int listeningPort, string peerFileName, VersionControl versionControl)
        {
			// Initialize version control
			_versionControl = versionControl;

            // Initialize peer list
            _peers = PeerList.GetPeerList(peerFileName);
			_listeningPort = listeningPort;

            // Initialize listener
			_listener = new AtomicMessageListener(listeningPort);
            _listener.OnReceive += OnReceiveMessage;

            // Initialize timer to connect other peers periodically
            System.Timers.Timer timer = new System.Timers.Timer(Config.GetConfig().ConnectPeriodMs);
            timer.Elapsed += this.OnTimerEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            // Initialize request manager
            _requestManager = new RequestManager();
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
