using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Distribox.FileSystem;

namespace Distribox.Network
{
    class RequestQueueItem
    {
        public AtomicPatch Patch { get; set; }
        public ArrayList PeerHaveThis { get; set; }

        public RequestQueueItem(AtomicPatch patch, ArrayList peerHaveThis)
        {
            Patch = patch;
            PeerHaveThis = peerHaveThis;
        }
    }
}
