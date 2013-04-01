using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Distribox.FileSystem;
using Distribox.CommonLib;

namespace Distribox.Network
{
    /// <summary>
    /// This class will
    /// * Decide which Patches to request right now
    /// * Manage request to ensure no request will be sent twice
    /// * Resend request if request isn't finish
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    class RequestManager
    {
        private Dictionary<AtomicPatch, HashSet<Peer>> _patchRequesting;
        private Dictionary<AtomicPatch, HashSet<Peer>> _patchToRequest;

        private void CheckForRequestExpire()
        {
            // TODO expire requests
        }

        public RequestManager()
        {
            _patchRequesting = new Dictionary<AtomicPatch, HashSet<Peer>>();
            _patchToRequest = new Dictionary<AtomicPatch, HashSet<Peer>>();
        }

        public void AddRequests(List<AtomicPatch> requests, Peer peerHaveThese)
        {
            lock (this)
            {
                Console.WriteLine("AddRequests :: {0}", requests.Count());
                foreach (AtomicPatch patch in requests)
                {
                    Console.WriteLine("\t{0}", patch.SerializeInline());
                    if (_patchRequesting.ContainsKey(patch))
                    {
                        Console.WriteLine("\tA");
                        _patchRequesting[patch].Add(peerHaveThese);
                    }
                    else if (_patchToRequest.ContainsKey(patch))
                    {
                        Console.WriteLine("\tB");
                        _patchToRequest[patch].Add(peerHaveThese);
                    }
                    else
                    {
                        Console.WriteLine("\tC");
                        var peers = new HashSet<Peer>();
                        peers.Add(peerHaveThese);
                        _patchToRequest.Add(patch, peers);
                    }
                }
            }
        }

        public Tuple<List<AtomicPatch>, Peer> GetRequests()
        {
            lock (this)
            {
                // Check this only at here should be enough
                CheckForRequestExpire();

                // Find a not requested patch
                AtomicPatch seedPatch = null;
                Peer peer = null;
                if (_patchToRequest.Count > 0)
                {
                    KeyValuePair<AtomicPatch, HashSet<Peer>> kvp = _patchToRequest.First<KeyValuePair<AtomicPatch, HashSet<Peer>>>();
                    seedPatch = kvp.Key;
                    peer = kvp.Value.First<Peer>();
                }
                if (seedPatch == null)
                    return null;

                // See if any other patches can be requested from the same peer
                HashSet<AtomicPatch> patches = new HashSet<AtomicPatch>();
                long requestSize = seedPatch.Size;
                // TODO improve time efficiency
                foreach (KeyValuePair<AtomicPatch, HashSet<Peer>> kvp in _patchToRequest)
                    if (kvp.Value.Contains(peer))
                    {
                        if (requestSize + kvp.Key.Size > Properties.MaxRequestSize)
                            break;

                        requestSize += kvp.Key.Size;
                        patches.Add(kvp.Key);
                    }

                // Move them to requesting set
                foreach (AtomicPatch patch in patches)
                {
                    // TODO: ugly!!!
                    Console.WriteLine("GetRequests::{0}", patch.SerializeInline());
                    _patchRequesting.Add(patch, _patchToRequest[patch]);
                    _patchToRequest.Remove(patch);
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
                    _patchRequesting.Remove(patch);
            }
            Console.WriteLine("requests:: {0}", requests.SerializeInline());
            Console.WriteLine("_patchRequesting:: {0}", _patchRequesting.SerializeInline());
        }
    }
}
