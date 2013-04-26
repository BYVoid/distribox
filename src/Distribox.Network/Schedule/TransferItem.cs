using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Network
{
    /// <summary>
    /// Used for `BandwidthEstimator`.
    /// </summary>
    internal struct TransferItem
    {
        public DateTime beginTime;
        public DateTime endTime;
        public int bytesPerSecond;
        public int ID;
        public Peer peer;
    }
}
