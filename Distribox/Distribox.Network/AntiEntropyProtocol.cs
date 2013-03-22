using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Distribox.Network
{
    using Distribox.CommonLib;

    class Session
    {
        public VersionList List;
    }

    class AntiEntropyProtocol
    {
        private const int CONNECT_PERIOD_MS = 1000;

        //private const string peerFileName = "peerlist.json";
        private PeerList _peers;
        private AtomicMessageListener _listener;
        private int _myPort;
        private VersionList _versionList;
        //private Dictionary<Peer, Session> sessions;

        private void ReceiveHandler(byte[] data, String address)
        {
            // Parse it, and convert to the right derived class
            ProtocolMessage message = CommonLib.CommonHelper.Read<ProtocolMessage>(data).ParseToDerivedClass(data);            

            // Parse IP and Port
            string[] ipAndPort = address.Split(':');
            string ip = ipAndPort[0];
            // ipAndPort[1] is the port of the sender socket, but we need the number of the listener port......
            int port = message.MyListenPort;
            Peer peer = new Peer(IPAddress.Parse(ip), port);

            // Process message
            if (message is Invitation) ProcessInvitation(peer);
            else if (message is AcceptInvitation) ProcessAcceptInvitation(peer);
            else if (message is ConnectRequest) ProcessConnectRequest(peer);
            else if (message is AcceptConnect) ProcessAcceptConnect(peer);
            else if (message is PeerListMessage) ProcessPeerList(peer, (PeerListMessage)message);
            else if (message is VersionListMessage) ProcessVersionList(peer, (VersionListMessage)message);
            else if (message is FileRequest) ProcessFileRequest(peer, (FileRequest)message);
            else if (message is FileDataResponse) ProcessFileResponse(peer, (FileDataResponse)message);
            else
            {
                throw new Exception("Receiver: Unseen message type!");
            }
        }

        private static void SendMessage(Peer peer, ProtocolMessage message)
        {
            AtomicMessageSender sender = new Network.AtomicMessageSender(peer.IP, peer.Port);
            byte[] bMessage = CommonLib.CommonHelper.ShowAsBytes(message);
            //Console.WriteLine(CommonLib.CommonHelper.ByteToString(bMessage));
            sender.SendBytes(bMessage);
        }

        private void ProcessInvitation(Peer peer)
        {
            /*
             * 1. Send AcceptInvivation back
             */
            SendMessage(peer, new AcceptInvitation(_myPort));
        }

        private void ProcessAcceptInvitation(Peer peer)
        {
            /*
             * 1. Try to Connect to that user
             */
            SendMessage(peer, new ConnectRequest(_myPort));
        }

        private void ProcessConnectRequest(Peer peer)
        {
            /*
             * Accept the Connect
             * Send MetaData
             * ...
             */
            SendMessage(peer, new AcceptConnect(_myPort));
            SendMetaData(peer);
        }

        private void ProcessAcceptConnect(Peer peer)
        {
            SendMetaData(peer);
        }

        private void ProcessPeerList(Peer peer, PeerListMessage peerListMessage)
        {
            lock (_peers)
            {
                _peers.AddPeerAndFlush(peer);
                _peers.MergeWith(peerListMessage.List);
            }
        }

        private void ProcessVersionList(Peer peer, VersionListMessage versionListMessage)
        {            
            List<FileItem> versionRequest;
            lock (_versionList)
            {
                versionRequest = _versionList.GetLessThan(versionListMessage.List);
            }            
            SendMessage(peer, new FileRequest(versionRequest, _myPort));
        }

        private void ProcessFileRequest(Peer peer, FileRequest request)
        {
            byte[] data;
            lock (_versionList)
            {
                 data = _versionList.CreateFileBundle(request._request);
            }            
            SendMessage(peer, new FileDataResponse(data, _myPort));
        }

        private void ProcessFileResponse(Peer peer, FileDataResponse response)
        {
            lock(_versionList)
            {
                _versionList.AcceptFileBundle(response._data);
            }
        }

        private void SendMetaData(Peer peer)
        {
            /*
             * 1. Send PeerList
             * 2. Send VersionList             
             */
            SendMessage(peer, new PeerListMessage(_peers, _myPort));
            SendMessage(peer, new VersionListMessage(_versionList, _myPort));
        }

        private void ConnectRandomPeer()
        {
            Peer peer;
            lock (_peers)
            {
                peer = _peers.SelectRandomPeer();
            }
            if (peer!=null)
                SendMessage(peer, new ConnectRequest(_myPort));
        }

        private void OnTimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            ConnectRandomPeer();
        }  

        public AntiEntropyProtocol(int myPort, string peerFileName)
        {
            // Initialize peer list
            _peers = PeerList.GetPeerList(peerFileName);
            _myPort = myPort;

            // Initialize listener
            _listener = new AtomicMessageListener(myPort);
            _listener.OnReceive += new AtomicMessageListener.OnReceiveHandler(ReceiveHandler);

            // Initialize timer to connect other peers periodically
            System.Timers.Timer timer = new System.Timers.Timer(CONNECT_PERIOD_MS);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerEvent);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void InvitePeer(Peer peer)
        {
            SendMessage(peer, new Invitation(_myPort));
        }
        
    }
}
