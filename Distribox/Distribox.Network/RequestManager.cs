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
    /// It maintains two queues in it: Todo queue and Doing queue, the lifecycle of a request is
    ///       getRequests         FinishRequests
    /// Todo -------------> Doing---------------->Done
    ///      <-------------
    ///       Fail / Expire
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
  
        /// <summary>
        /// Adds the requests.
        /// </summary>
        /// <param name='requests'>
        /// Requests to add.
        /// </param>
        /// <param name='peerHaveThese'>
        /// Peer who have these patches.
        /// </param>
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
  
        /// <summary>
        /// Gets some requests to be send to a peer. The requests will not exceed <see cref="Distribox.CommonLib.Properties.MaxRequestSize"/>
        /// </summary>
        /// <returns>
        /// The requests.
        /// </returns>
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
  
        /// <summary>
        /// Finishs the requests when response of these requests are received.
        /// </summary>
        /// <param name='requests'>
        /// Requests to be finished.
        /// </param>
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
  
        /// <summary>
        /// Every time before GetRequests, this method will be invoked to move expired requests from Doing queue back to Todo queue.
        /// </summary>
        private void CheckForRequestExpire()
        {
            // TODO expire requests
        }
        
        /// <summary>
        /// Fails the requests.
        /// </summary>
        /// <param name='requests'>
        /// Requests to be moved back to Todo queue.
        /// </param>
        /// <exception cref='NotImplementedException'>
        /// Is thrown when a requested operation is not implemented for a given type.
        /// </exception>
        public void FailRequests(List<AtomicPatch> requests)
        {
            throw new NotImplementedException();
        }
    }
}
