//-----------------------------------------------------------------------
// <copyright file="RequestManager.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Distribox.CommonLib;
    using Distribox.FileSystem;

    // TODO use max number of connections instead of max "bandwidth"

    /// <summary>
    /// The manager of sending patches
    /// RequestManager will
    /// * Decide which Patches to request right now
    /// * Manage request to ensure no request will be sent twice
    /// * Resend request if request isn't finished
    /// It maintains two queues in it: TODO queue and Doing queue, the lifecycle of a request is
    ///       getRequests         FinishRequests
    /// TODO -------------> Doing ----------------> Done
    ///                           ----------------> Fail / Expire
    /// </summary>
    /// <remarks>
    /// <para />
    /// </remarks>
    public class RequestManager
    {
        private Dictionary<FileEvent, List<Peer>> todoFileToPeer;

        private Dictionary<Peer, List<FileEvent>> todoPeerToFile;

        private List<DoingQueueItem> doing;

        private BandwidthEstimator estimator;

        private int usedBandwidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.Network.RequestManager"/> class.
        /// </summary>
        public RequestManager()
        {
            this.todoFileToPeer = new Dictionary<FileEvent, List<Peer>>();
            this.todoPeerToFile = new Dictionary<Peer,List<FileEvent>>();
            this.doing = new List<DoingQueueItem>();
            this.estimator = new BandwidthEstimator();
            this.usedBandwidth = 0;
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
                if (!todoPeerToFile.ContainsKey(peerHaveThese))
                    todoPeerToFile[peerHaveThese] = new List<FileEvent>();

                todoPeerToFile[peerHaveThese].AddRange(requests);

                foreach (FileEvent patch in requests)
                {
                    if (!todoFileToPeer.ContainsKey(patch))
                        todoFileToPeer[patch] = new List<Peer>();

                    todoFileToPeer[patch].Add(peerHaveThese);
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
            CheckForRequestExpire();
            lock (this)
            {
                // No requests
                if (this.todoFileToPeer.Count() == 0)
                {
                    return null;
                }

                Peer bestPeer = new Peer();
                List<FileEvent> bestCollection = new List<FileEvent>();
                double bestScore = 0;
                long bestSize = 0;

                // For each peer
                foreach (KeyValuePair<Peer, List<FileEvent>> ppf in this.todoPeerToFile)
                {
                    Peer peer = ppf.Key;                    
                    var slist = ppf.Value.OrderBy(x => x.When.Ticks).ToList();

                    // Find file collections
                    double score = Properties.RMBandwidthWeight * this.estimator.GetPeerBandwidth(peer);

                    long currentSize = 0;
                    List<FileEvent> currentCollection = new List<FileEvent>();
                    foreach (FileEvent f in slist)
                    {
                        if (currentSize + f.Size <= Properties.MaxRequestSize)
                        {
                            currentSize += f.Size;
                            currentCollection.Add(f);

                            double uniqueness = 1.0 / this.todoFileToPeer[f].Count();
                            score += Properties.RMUniquenessWeight * uniqueness;
                        }
                        else
                        {
                            break;
                        }
                    }
                    score += Properties.RMSizeWeight * currentSize;

                    // Update best solution
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPeer = peer;
                        bestCollection = currentCollection;
                        bestSize = currentSize;
                    }
                }
                
                // Do we have enough bandwidth ?
                int needBandwidth = this.estimator.GetPeerBandwidth(bestPeer);
                if (needBandwidth == 0)
                {
                    needBandwidth = Properties.DefaultConnectionSpeed;
                }

                if (usedBandwidth + needBandwidth > Properties.DefaultBandwidth)
                {
                    return null;
                }

                // Do this!
                Console.WriteLine("Request");
                usedBandwidth += needBandwidth;

                DoingQueueItem dqItem = new DoingQueueItem();
                dqItem.bandWidth = needBandwidth;
                dqItem.expireDate = DateTime.Now.AddSeconds(bestSize * Properties.ExpireSlackCoefficient / needBandwidth + Properties.DefaultDelay);                
                dqItem.files = new List<DoingQueueFileItem>();

                foreach (FileEvent file in bestCollection)
                {
                    DoingQueueFileItem fileItem = new DoingQueueFileItem();
                    fileItem.file = file;
                    fileItem.whoHaveMe = this.todoFileToPeer[file];

                    // Remove File->Peer
                    this.todoFileToPeer.Remove(file);

                    // Remove Peer->File
                    foreach (Peer peer in fileItem.whoHaveMe)
                    {
                        this.todoPeerToFile[peer].Remove(file);
                    }

                    dqItem.files.Add(fileItem);
                }

                // Add it
                dqItem.filesHash = CommonHelper.GetHashCode(bestCollection);
                this.doing.Add(dqItem);
                this.estimator.BeginRequest(bestPeer, dqItem.filesHash, (int)bestSize);

                // return success
                return new Tuple<List<FileEvent>,Peer>(bestCollection, bestPeer);
            }
        }

        /// <summary>
        /// Remove item from doing queue, if not success, insert them back to the
        /// todo queue.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="success"></param>
        public void FinishRequests(DoingQueueItem item, bool success)
        {
            Console.WriteLine("Finish {0}", success);
            this.usedBandwidth -= item.bandWidth;
            this.doing.Remove(item);

            if (!success)
            {
                // Add them back to todo (merge)
                foreach (DoingQueueFileItem fileItem in item.files)
                {
                    if (!this.todoFileToPeer.ContainsKey(fileItem.file))
                    {
                        this.todoFileToPeer[fileItem.file] = new List<Peer>();
                    }

                    foreach (Peer peer in fileItem.whoHaveMe)
                    {
                        // Maintain Peer->File
                        if (this.todoPeerToFile[peer].IndexOf(fileItem.file) == -1)
                            this.todoPeerToFile[peer].Add(fileItem.file);
                        
                        // Maintain File->Peer
                        if (this.todoFileToPeer[fileItem.file].IndexOf(peer) == -1)
                            this.todoFileToPeer[fileItem.file].Add(peer);
                    }
                }
            }
        }

        public void FinishRequests(List<FileEvent> requests)
        {
            int hash = CommonHelper.GetHashCode(requests);

            this.estimator.FinishRequest(hash);
            lock (this)
            {
                foreach (DoingQueueItem item in this.doing)
                {
                    if (item.filesHash == hash)
                    {
                        this.estimator.FinishRequest(item.filesHash);
                        FinishRequests(item, true);
                        break;
                    }
                }

                Logger.Warn("Finishing not existing requests.");                
            }
        }
        
        /// <summary>
        /// Fails the requests.
        /// </summary>
        /// <param name='requests'>
        /// Requests to be moved back to TODO queue.
        /// </param>
        /// <exception cref='NotImplementedException'>
        /// Is thrown when a requested operation is not implemented for a given type.
        /// </exception>
        public void FailRequests(List<FileEvent> requests)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Every time before GetRequests, this method will be invoked to move expired requests from Doing queue back to TODO queue.
        /// </summary>
        private void CheckForRequestExpire()
        {
            lock (this)
            {
                DateTime now = DateTime.Now;
                List<DoingQueueItem> expired = new List<DoingQueueItem>();

                foreach (DoingQueueItem item in this.doing)
                {
                    if (now > item.expireDate)
                    {
                        expired.Add(item);
                    }
                }

                foreach (DoingQueueItem item in expired)
                {
                    this.estimator.FailRequest(item.filesHash);
                    FinishRequests(item, false);
                }
            }
        }
    }
}
