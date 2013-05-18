//-----------------------------------------------------------------------
// <copyright file="AntiEntropyProtocol.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Distribox.CommonLib;
    using Distribox.FileSystem;

    /// <summary>
    /// The implementation of Anti-entropy protocol.
    /// <para />
    /// Anti-entropy protocol is first purposed in 1980s, using for maintaining replicated databases all over the world. According to literature, it is "extremely robust" [1]. 
    /// Generally, there are two kind of approaches to synchronize data: the reactive approach and epidemic approach. In the former one, when some change happen, the peer changed acknowledge other peers and these peers get changed too. These newly changed peer continue acknowledging peers, until the change is spread to the whole network. This protocol is not robust, since connection error may happen any time, much effort is needed to prevent potential factors that leads spreading stop before changes are spread. For example, all the peers with change are isolated with other peers at some moment.
    /// Instead of passively receive changes, epidemic approach try to synchronize with other nodes consistently. Implementing such protocols involves much less consideration than radioactive approaches.
    /// <para />
    /// Reference
    /// [1] Demers, Alan, et al. "Epidemic algorithms for replicated database maintenance." Proceedings of the sixth annual ACM Symposium on Principles of distributed computing. ACM, 1987.
    /// </summary>
    public class AntiEntropyProtocol
    {
        /// <summary>
        /// The peers.
        /// </summary>
        private PeerList peers;

        /// <summary>
        /// The listener.
        /// </summary>
        private AtomicMessageListener listener;

        /// <summary>
        /// The listening port.
        /// </summary>
        private int listeningPort;

        /// <summary>
        /// The version control.
        /// </summary>
        private VersionControl versionControl;

        /// <summary>
        /// Use File Event as its member
        /// </summary>
        private RequestManager requestManager;

        /// <summary>
        /// Factory class for ProtocolMessage
        /// </summary>
        private ProtocolMessageFactory messageFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.AntiEntropyProtocol"/> class.
        /// </summary>
        /// <param name="listeningPort">Listening port.</param>
        /// <param name="peerFileName">Peer file name.</param>
        /// <param name="versionControl">Version control.</param>
        public AntiEntropyProtocol(int listeningPort, string peerFileName, VersionControl versionControl)
        {
            // Initialize version control
            this.versionControl = versionControl;

            // Initialize peer list
            this.peers = PeerList.GetPeerList(peerFileName);
            this.listeningPort = listeningPort;

            // Initialize listener
            this.listener = new AtomicMessageListener(listeningPort);
            this.listener.OnReceive += this.OnReceiveMessage;

            // Initialize timer to connect other peers periodically
            System.Timers.Timer timer = new System.Timers.Timer(Config.ConnectPeriodMs);
            timer.Elapsed += this.OnTimerEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            // Initialize request manager
            this.requestManager = new RequestManager();

            // Initialize Protocol message factory
            this.messageFactory = new ProtocolMessageFactory();
        }

        /// <summary>
        /// Invites a peer into P2P network.
        /// </summary>
        /// <param name="peer">The peer to be invited.</param>
        public void InvitePeer(Peer peer)
        {
            SendMessage(peer, new InvitationRequest(this.listeningPort));
        }

        /// <summary>
        /// Process the invitation message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="peer">The peer.</param>
        internal void Process(InvitationRequest message, Peer peer)
        {
            // Send AcceptInvivation back
            SendMessage(peer, new InvitationAck(this.listeningPort));
        }

        /// <summary>
        /// Process the accept message and peer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="peer">The peer.</param>
        internal void Process(InvitationAck message, Peer peer)
        {
            // Try to sync with the newly accepted peer
            SendMessage(peer, new SyncRequest(this.listeningPort));
        }

        /// <summary>
        /// Process the sync message and peer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="peer">The peer.</param>
        internal void Process(SyncRequest message, Peer peer)
        {
            // Accept the sync request
            SendMessage(peer, new SyncAck(this.listeningPort));

            // Send MetaData
            this.SendMetaData(peer);
        }

        /// <summary>
        /// Process the specified message and peer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="peer">The peer.</param>
        internal void Process(SyncAck message, Peer peer)
        {
            this.SendMetaData(peer);
        }

        /// <summary>
        /// Process the peer list message and peer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="peer">The peer.</param>
        internal void Process(PeerListMessage message, Peer peer)
        {
            lock (this.peers)
            {
                this.peers.AddPeer(peer);
                this.peers.MergeWith(message.List);
            }
        }

        /// <summary>
        /// Process the version list message and peer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="peer">The peer.</param>
        internal void Process(VersionListMessage message, Peer peer)
        {
            List<FileEvent> versionRequest;
            lock (this.versionControl.VersionList)
            {
                versionRequest = this.versionControl.VersionList.GetLessThan(message.List);
                Logger.Info("Received version list from {1}\n{0}", message.List.Serialize(), peer.Serialize());
            }

            // Don't request them now, add them to a request queue first
            this.requestManager.AddRequests(versionRequest, peer);
            this.TryToRequest();
        }

        /// <summary>
        /// Process the file request message and peer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="peer">The peer.</param>
        internal void Process(PatchRequest message, Peer peer)
        {
            Logger.Info("Receive file request\n{0}", message.Request.Serialize());
            byte[] data = VersionControl.CreateFileBundle(message.Request);
            SendMessage(peer, new FileDataResponse(data, this.listeningPort));
        }

        /// <summary>
        /// Process the specified message and peer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="peer">The peer.</param>
        internal void Process(FileDataResponse message, Peer peer)
        {
            List<FileEvent> patches;
            lock (this.versionControl.VersionList)
            {
                patches = this.versionControl.AcceptFileBundle(message.Data);
            }

            this.requestManager.FinishRequests(patches);
            this.TryToRequest();
        }

        /// <summary>
        /// Sends a message to peer.
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <param name="message">The message.</param>
        /// <param name="onCompleteHandler">On complete handler.</param>
        private static void SendMessage(Peer peer, ProtocolMessage message, AtomicMessageSender.OnCompleteHandler onCompleteHandler = null)
        {
            if (peer == null)
            {
                return;
            }

            AtomicMessageSender sender = new Network.AtomicMessageSender(peer);
            if (onCompleteHandler != null)
            {
                sender.OnComplete += onCompleteHandler;
            }

            byte[] bytesMessage = new ProtocolMessageContainer(message).SerializeAsBytes();
            sender.SendBytes(bytesMessage);

            // TODO on error
        }

        /// <summary>
        /// Handle receive message event.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="peerFrom">peer from.</param>
        private void OnReceiveMessage(byte[] data, Peer peerFrom)
        {
            this.ParseAndDispatchMessage(data, peerFrom);
        }

        /// <summary>
        /// Parses and dispatch message from peer.
        /// </summary>
        /// <param name="data">The ata.</param>
        /// <param name="peerFrom">The address.</param>
        private void ParseAndDispatchMessage(byte[] data, Peer peerFrom)
        {
            // Parse it, and convert to the right derived class
            var container = CommonHelper.Deserialize<ProtocolMessageContainer>(data);
            ProtocolMessage message = this.messageFactory.CreateMessage(container);

            // ipAndPort[1] is the port of the sender socket, but we need the number of the listener port......
            int port = message.ListeningPort;
            Peer peer = new Peer(peerFrom.IP, port);

            // Process message (visitor design pattern)
            message.Accept(this, peer);
        }

        /// <summary>
        /// Repeatedly get and send new requests, until `RequestManager` don't give us any more requests. (May because       
        /// there are no more requests or request manager thinks there are too many patches requesting now).
        /// </summary>
        private void TryToRequest()
        {
            while (true)
            {
                // Get a request from RequestManager
                var requestTuple = this.requestManager.GetRequests();

                // Maybe there are not any requests...
                if (requestTuple == null)
                {
                    return;
                }

                List<FileEvent> patches = requestTuple.Item1;
                Peer peer = requestTuple.Item2;

                // Send out the request
                SendMessage(peer, new PatchRequest(patches, this.listeningPort));
            }
        }

        /// <summary>
        /// Sends the meta data.
        /// </summary>
        /// <param name="peer">The peer.</param>
        private void SendMetaData(Peer peer)
        {
            // Send PeerList
            SendMessage(peer, new PeerListMessage(this.peers, this.listeningPort));

            // Send VersionList
            SendMessage(peer, new VersionListMessage(this.versionControl.VersionList, this.listeningPort));
        }

        /// <summary>
        /// Connects a random peer to send a connection request.
        /// </summary>
        private void ConnectRandomPeer()
        {
            if (this.peers.Peers.Count() == 0)
            {
                return;
            }

            Peer peer;

            // Lock peers to support thread-safety
            lock (this.peers)
            {
                // This might select itself, whatever, we'll accept it.
                peer = this.peers.SelectRandomPeer();
            }

            if (peer != null)
            {
                SendMessage(peer, new SyncRequest(this.listeningPort));
            }
        }

        /// <summary>
        /// Handle timer event.
        /// </summary>
        /// <param name="source">Source of event.</param>
        /// <param name="e">The event.</param>
        private void OnTimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            this.ConnectRandomPeer();
        }
    }
}
