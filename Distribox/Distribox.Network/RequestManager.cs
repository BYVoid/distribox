namespace Distribox.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Distribox.CommonLib;
    using Distribox.FileSystem;

    /// <summary>
    /// The manager of sending patches
    /// RequestManager will
    /// * Decide which Patches to request right now
    /// * Manage request to ensure no request will be sent twice
    /// * Resend request if request isn't finished
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    internal class RequestManager
    {
        private Dictionary<AtomicPatch, HashSet<Peer>> patchRequesting;
        private Dictionary<AtomicPatch, HashSet<Peer>> patchToRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.RequestManager"/> class.
        /// </summary>
        public RequestManager()
        {
            this.patchRequesting = new Dictionary<AtomicPatch, HashSet<Peer>>();
            this.patchToRequest = new Dictionary<AtomicPatch, HashSet<Peer>>();
        }

        public void AddRequests(List<AtomicPatch> requests, Peer peerHaveThese)
        {
            lock (this)
            {
                Console.WriteLine("AddRequests :: {0}", requests.Count());
                foreach (AtomicPatch patch in requests)
                {
                    Console.WriteLine("\t{0}", patch.SerializeInline());
                    if (this.patchRequesting.ContainsKey(patch))
                    {
                        Console.WriteLine("\tA");
                        this.patchRequesting[patch].Add(peerHaveThese);
                    }
                    else if (this.patchToRequest.ContainsKey(patch))
                    {
                        Console.WriteLine("\tB");
                        this.patchToRequest[patch].Add(peerHaveThese);
                    }
                    else
                    {
                        Console.WriteLine("\tC");
                        var peers = new HashSet<Peer>();
                        peers.Add(peerHaveThese);
                        this.patchToRequest.Add(patch, peers);
                    }
                }
            }
        }

        public Tuple<List<AtomicPatch>, Peer> GetRequests()
        {
            lock (this)
            {
                // Check this only at here should be enough
                this.CheckForRequestExpire();

                // Find a not requested patch
                AtomicPatch seedPatch = null;
                Peer peer = null;
                if (this.patchToRequest.Count > 0)
                {
                    KeyValuePair<AtomicPatch, HashSet<Peer>> kvp = this.patchToRequest.First<KeyValuePair<AtomicPatch, HashSet<Peer>>>();
                    seedPatch = kvp.Key;
                    peer = kvp.Value.First<Peer>();
                }

                if (seedPatch == null)
                {
                    return null;
                }

                // See if any other patches can be requested from the same peer
                HashSet<AtomicPatch> patches = new HashSet<AtomicPatch>();
                long requestSize = seedPatch.Size;
                // TODO improve time efficiency
                foreach (KeyValuePair<AtomicPatch, HashSet<Peer>> kvp in this.patchToRequest)
                {
                    if (kvp.Value.Contains(peer))
                    {
                        if (requestSize + kvp.Key.Size > Properties.MaxRequestSize)
                        {
                            break;
                        }

                        requestSize += kvp.Key.Size;
                        patches.Add(kvp.Key);
                    }
                }

                // Move them to requesting set
                foreach (AtomicPatch patch in patches)
                {
                    // TODO: ugly!!!
                    Console.WriteLine("GetRequests::{0}", patch.SerializeInline());
                    this.patchRequesting.Add(patch, this.patchToRequest[patch]);
                    this.patchToRequest.Remove(patch);
                }
                // Return
                return Tuple.Create<List<AtomicPatch>, Peer>(patches.ToList(), peer);
            }
        }

        public void FinishRequests(List<AtomicPatch> requests)
        {
            // Remove them from _patchRequesting
            lock (this)
            {
                foreach (AtomicPatch patch in requests)
                {
                    this.patchRequesting.Remove(patch);
                }
            }

            Console.WriteLine("requests:: {0}", requests.SerializeInline());
            Console.WriteLine("_patchRequesting:: {0}", this.patchRequesting.SerializeInline());
        }

        private void CheckForRequestExpire()
        {
            // TODO expire requests
        }
    }
}
