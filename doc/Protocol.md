# Protocol

## Overview
<DEPRECATED>
There are 2 kind of protocols, Distribox uses a hybrid approach of both:

Anti-entropy: Each peer pick a random peer regularly and these 2 nodes completely synchronize their data. This approach is extremely reliable but propagates changes slowly. This approach is suitable when significant difference of peers exists, for example, a newly-invited peer or a peer which has just login.
Gossip protocol: Rather than synchronizing data completely, gossip protocol spreads updates (as rumor). When a peer receive a new rumor, the rumor becomes hot. If a peer sends a rumor to other peers and is acknowledged that the rumor has already been seen, the rumor will becomes cold. Rumor cycles can be more frequent than anti-entropy cycles because they require fewer resources at each site, but there is some chance that an update will not reach all sites.
These ideas exists since 1987[1], by the propose for replicated database maintenance in Xerox company. Since Distribox can be considered as a replicated database, where versions in Distribox are considered as records in database, we found that the ideas are still applicable to Distribox without major modifications. The major difference these two scenarios perhaps are

Connection speed
Network stability
</DEPRECATED>

## Storage organization
The file system (.dbox folder) can be organized with hierarchies:

* Versions: a version will include
 * Timestamp: the time of the commit, in GMT. If this time is incorrect or asynchronized among peers. Distribox may respect wrong file as the latest. This mistake could be repaired by manually set the correct latest version. (We haven't considered how to spread this correction to other peers yet.)
 * Commit ID: a 160-bit random number, generated when commit is submitted on the peer who made the commit.
 * File list: a xml file, contains full path and filename of all files in this commit, and number of file blocks in this version.
 * File blocks: 1MB sized blocks, named by Hash(file name + block ID in this file), where Hash(x) is the 160-bit SHA1 code of x. The block data contains: Commit ID + 1 MB binary data of the original file.

## Anti-entropy
To implement anti-entropy, each peer should has two threads: the pull thread, waiting for other thread’s synchronization and the push thread, which try start synchronization with a random peer actively every t milliseconds.

    Push-thread(peer p)
    	For every t milliseconds
    		If p is idle
    			q = random peer from peer list
    			Send “Invitation” to q
    			If q returned “Accept”
    				set p is busy
    				Synchronize-actively(p, q)
    				set p is idle
    			Else
    				do nothing

    Pull-thread(peer q)
    	Wait for invitation, suppose invitation is from peer p
    		If q is busy
    			Send “Refuse”
    		Else 					# q is idle
    			set q is busy
    			Send “Accept”
    			Synchronize-passively(p, q)
    			set q is idle

According to [1], suppose no more updates are made since time 0. The probability that a peer is NOT synchronized at time t, denoting it as pt , follows the recursive equation.
pt+1 = pt^2

For arbitrary small value ε, the expected number of t, such that pt<ε, is proportional to O(log* p0/ε).

## Incremental synchronization

<DEPRECATED>

The efficiency of anti-entropy is relies on the efficiency of synchronization. If two .dbox folders is compared directed during a single synchronization, the efficiency will be very low and the system cannot be used in practice. Our ideal result is during each synchronization, only differences of two versions is transferred, that is the ideal incremental synchronization. In practice, additional information should be transferred to found the difference between .dbox folders.

Our approach is a hierarchical hash algorithm. First, we sort all data, namely, file blocks and file lists by their timestamp followed by their hash. We do this not only for define a unique and deterministic sequence for files, but also try to make latest versions aggregate in our hash tree (explained soon), reducing the expensive network I/Os.

For the i th item, denote its hash as hashi. We then compute Hash(hash0, hash1, …, hashb-1), Hash(hashb+0, hashb+1, …, hash2b-1) for a branch constant b. And keep doing this until we got a final hash code for the root node.

Every time when synchronization begins, the active thread send its hash of root to passive thread, the passive thread will compare it with its own hash of root to decide if synchronization should be done on other nodes

</DEPRECATED>

    Synchronize-actively(peer p, peer q)
    	Send user list to q
    	Receive user list from q
    	Merge

    	Send commit ID list to q
    	Receive commit ID list from q
    	Merge received commit ID list with its own list

    	For any commit ID received from q
    		Send q “Send commit ID”
    		Receive data from q

      Send q “Sync 1”
      Keep received instructions from q until received “Sync 2”
      	Send commit ID to q

      For any commit ID that hash of p and q conflicts
      	Send file list to q
      	Receive file list from q
      	For any file block that p have but q doesn’t have
      		Send that block to q
      	For any file block that q have but p doesn’t have
      		Receive that block from q

    Safe-receive(peer p, peer q)
    	q receives file_block from p
      q receives hash from p
      	If Hash(file_block) is equal to hash
      		q saves file_block
      	Else
          Safe-receive(peer p, peer q)

    Check-If-Version-Complete(file_block)
    	save(file_block.data)
      received_commit_id = file_block.id
      	If every block of received_commit_id is received
      		Merge all blocks of received_commit_id
      		Update files locally by received_commit_id

## Invitation and Online/Offline
### Invitation
If p want to invite q, p and q do a synchronization. Then p and q add q to user list.

### Online
When a peer online. It does nothing and wait for a synchronization or synchronize with others.

### Offline
We can’t do anything when peer offline. The user list is not modified, peers may still try to connect offline nodes.

## Reference
[1] Demers, Alan, et al. "Epidemic algorithms for replicated database maintenance."Proceedings of the sixth annual ACM Symposium on Principles of distributed computing. ACM, 1987.
