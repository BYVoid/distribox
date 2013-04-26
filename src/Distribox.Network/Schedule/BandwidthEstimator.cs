using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Network
{
    using System.IO;
    using Distribox.CommonLib;

    /// <summary>
    /// This class provide methods to estimate bandwidth between local machine and
    /// peers. Upload / download asymmetry is not considered yet.    
    /// 
    /// Currently, peer bandwidth is measured by simply compute the max transfer size / time
    /// ratio of single block. Blocks be transferred simutaneously between a pair of peer is
    /// not considered.
    /// # Thread safety
    /// Not thread safe.
    /// </summary>    
    internal class BandwidthEstimator
    {
        /// <summary>
        /// Queue of transferring items that is useful for computing total bandwidth.
        /// Sorted by endTime.
        /// </summary>
        private Queue<TransferItem> transferQueue;
        private Dictionary<int, TransferItem> ongoingTransfer;
        private Dictionary<Peer, int> peerBandwidth;        
        private int totalBytesPerSecond;        

        private void Flush()
        {
            // Serializing the dictionary directly will cause trouble
            List<KeyValuePair<Peer, int>> temp = new List<KeyValuePair<Peer,int>>();
            foreach ( KeyValuePair<Peer, int> pair in this.peerBandwidth)
            {
                temp.Add(pair);
            }
            temp.WriteObject(Config.PeerBandwidthFilePath);
        }

        private void UpdateTotalBandwidth()
        {
            this.totalBytesPerSecond = 0;
            foreach (TransferItem item in this.transferQueue)
            {
                // We are sure that item.endTime >= item0.beginTime
                this.totalBytesPerSecond += item.bytesPerSecond;
            }
        }

        private void PopQueueItems(TransferItem item0)
        {
            while (this.transferQueue.Peek().endTime < item0.beginTime)
            {
                this.transferQueue.Dequeue();
            }
        }

        private void FinishRequest(int hash, int factor)
        {
            if (!this.ongoingTransfer.ContainsKey(hash))
            {
                Logger.Warn("BandwidthEstimator: Not find transfer {0} in ongoingTransfer", hash);
                return;
            }

            // Push new item
            TransferItem item = this.ongoingTransfer[hash];
            this.ongoingTransfer.Remove(hash);

            item.endTime = DateTime.Now;
            int timeInterval = item.endTime.Second - item.beginTime.Second;
            if (timeInterval <= 0)
            {
                timeInterval = 1;
            }

            item.bytesPerSecond /= timeInterval;
            item.bytesPerSecond *= factor;

            this.peerBandwidth[item.peer] = item.bytesPerSecond;

            this.transferQueue.Enqueue(item);

            // Update
            this.PopQueueItems(item);
            this.UpdateTotalBandwidth();            
        }

        public BandwidthEstimator()
        {
            string peerBandwidthPath = Config.PeerBandwidthFilePath;
            if (!File.Exists(peerBandwidthPath))
            {
                Logger.Warn("Peer bandwidth meta data not found in {0}, creating a new one...", peerBandwidthPath);
                (new List<KeyValuePair<Peer, int>>()).WriteObject(peerBandwidthPath);
            }

            this.peerBandwidth = new Dictionary<Peer, int>();
            this.totalBytesPerSecond = Config.DefaultBandwidth;

            this.transferQueue = new Queue<TransferItem>();
            this.ongoingTransfer = new Dictionary<int, TransferItem>();

            // Deserializing the dictionary directly will cause trouble
            List<KeyValuePair<Peer, int>> temp;
            temp = CommonHelper.ReadObject<List<KeyValuePair<Peer, int>>>(peerBandwidthPath);
            foreach (KeyValuePair<Peer, int> pair in temp)
            {
                peerBandwidth[pair.Key] = pair.Value;
            }
        }

        ~BandwidthEstimator()
        {
            this.Flush();
        }

        public int TotalBandwidth
        {
            get
            {
                return totalBytesPerSecond;
            }
        }

        public int GetPeerBandwidth(Peer peer)
        {
            if (!this.peerBandwidth.ContainsKey(peer))
            {
                return 0;
            }

            return this.peerBandwidth[peer];
        }

        public void BeginRequest(Peer peer, int hash, int size)
        {
            if (ongoingTransfer.ContainsKey(hash))
            {
                Logger.Warn("BandwidthEstimator: Transfer {0} already exists ongoingTransfer");
                return;
            }

            TransferItem transfer = new TransferItem();
            transfer.ID = hash;
            transfer.bytesPerSecond = size;      // hacky
            transfer.beginTime = DateTime.Now;
            transfer.peer = peer;

            ongoingTransfer[hash] = transfer;
        }

        public void FinishRequest(int hash)
        {
            this.FinishRequest(hash, 1);
            Flush();
        }

        public void FailRequest(int hash)
        {
            // Byterate = 0 bytes / s
            this.FinishRequest(hash, 0);
            Flush();
        }
    }
}
