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
        private Dictionary<FileEvent, HashSet<Peer>> patchRequesting;
        private Dictionary<FileEvent, HashSet<Peer>> patchToRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.RequestManager"/> class.
        /// </summary>
        public RequestManager()
        {
            this.patchRequesting = new Dictionary<FileEvent, HashSet<Peer>>();
            this.patchToRequest = new Dictionary<FileEvent, HashSet<Peer>>();
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
        public void AddRequests(List<FileEvent> requests, Peer peerHaveThese)
        {
            lock (this)
            {
                foreach (FileEvent patch in requests)
                {
                    if (this.patchRequesting.ContainsKey(patch))
                    {
                        this.patchRequesting[patch].Add(peerHaveThese);
                    }
                    else if (this.patchToRequest.ContainsKey(patch))
                    {
                        this.patchToRequest[patch].Add(peerHaveThese);
                    }
                    else
                    {
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
        public Tuple<List<FileEvent>, Peer> GetRequests()
        {
            lock (this)
            {
                // Check this only at here should be enough
                this.CheckForRequestExpire();

                // Find a not requested patch
                FileEvent seedPatch = null;
                Peer peer = null;
                if (this.patchToRequest.Count > 0)
                {
                    KeyValuePair<FileEvent, HashSet<Peer>> kvp = this.patchToRequest.First<KeyValuePair<FileEvent, HashSet<Peer>>>();
                    seedPatch = kvp.Key;
                    peer = kvp.Value.First<Peer>();
                }

                if (seedPatch == null)
                {
                    return null;
                }

                // See if any other patches can be requested from the same peer
                HashSet<FileEvent> patches = new HashSet<FileEvent>();
                long requestSize = seedPatch.Size;
                // TODO improve time efficiency
                foreach (KeyValuePair<FileEvent, HashSet<Peer>> kvp in this.patchToRequest)
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
                foreach (FileEvent patch in patches)
                {
                    // TODO: ugly!!!
                    this.patchRequesting.Add(patch, this.patchToRequest[patch]);
                    this.patchToRequest.Remove(patch);
                }
                // Return
                return Tuple.Create<List<FileEvent>, Peer>(patches.ToList(), peer);
            }
        }
  
        /// <summary>
        /// Finishs the requests when response of these requests are received.
        /// </summary>
        /// <param name='requests'>
        /// Requests to be finished.
        /// </param>
        public void FinishRequests(List<FileEvent> requests)
        {
            // Remove them from _patchRequesting
            lock (this)
            {
                foreach (FileEvent patch in requests)
                {
                    this.patchRequesting.Remove(patch);
                }
            }
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
        public void FailRequests(List<FileEvent> requests)
        {
            throw new NotImplementedException();
        }
    }
}
