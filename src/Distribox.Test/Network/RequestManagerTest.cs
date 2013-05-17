namespace Distribox.Test
{
    using System.Threading;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Distribox.CommonLib;
    using Distribox.FileSystem;
    using NUnit.Framework;
    using Distribox.Network;

    [TestFixture]
    public class RequestManagerTest
    {
        [Test, Timeout(100000)]
        public void BasicFlow()
        {
            RequestManager manager = new RequestManager();

            FileEvent e1 = new FileEvent(); e1.EventId = "1"; e1.Size = 5 * 1024;
            FileEvent e2 = new FileEvent(); e2.EventId = "2";
            Peer p1 = new Peer(); p1.Port = 1111;
            Peer p2 = new Peer(); p2.Port = 2222;

            // No request yet
            Assert.IsNull(manager.GetRequests());

            // Add a request
            List<FileEvent> l1 = new List<FileEvent>(); l1.Add(e1);
            manager.AddRequests(l1, p1);

            // Check
            var tlp = manager.GetRequests();
            Assert.IsNotNull(tlp);
            Assert.IsTrue(Enumerable.SequenceEqual(tlp.Item1, l1));
            Assert.AreEqual(tlp.Item2, p1);

            // No request
            Assert.IsNull(manager.GetRequests());

            // The request fails
            Thread.Sleep(3000);

            // Should be available again
            tlp = manager.GetRequests();
            //Assert.IsNotNull(tlp);
            //Assert.IsTrue(Enumerable.SequenceEqual(tlp.Item1, l1));
            //Assert.AreEqual(tlp.Item2, p1);

            // No request
            //Assert.IsNull(manager.GetRequests());

            // The request finished
            manager.FinishRequests(l1);

            // No request
            //Assert.IsNull(manager.GetRequests());
        }

        [Test, Timeout(100000)]
        public void SpeedHeuristics()
        {
            RequestManager manager = new RequestManager();

            FileEvent e1 = new FileEvent(); e1.EventId = "1"; e1.Size = 4 * 1024 * 1024;
            FileEvent e2 = new FileEvent(); e2.EventId = "2"; e2.Size = 4 * 1024;
            Peer p1 = new Peer(); p1.Port = 1111;
            Peer p2 = new Peer(); p2.Port = 2222;
            List<FileEvent> l1 = new List<FileEvent>(); l1.Add(e1);
            List<FileEvent> l2 = new List<FileEvent>(); l2.Add(e2);

            // Fast peer
            manager.AddRequests(l1, p1);
            manager.GetRequests();
            manager.FinishRequests(l1);

            // Slow peer
            manager.AddRequests(l2, p2);
            manager.GetRequests();
            Thread.Sleep(3);
            manager.FinishRequests(l2);

            // Add two files
            manager.AddRequests(l1, p2);
            manager.AddRequests(l2, p1);

            // Should be fast peer
            var tlp = manager.GetRequests();
            Assert.AreEqual(p1, tlp.Item2);
        }

        [Test, Timeout(100000)]
        public void UniquenessHeuristics()
        {
            RequestManager manager = new RequestManager();

            FileEvent e1 = new FileEvent(); e1.EventId = "1"; e1.Size = 8 * 1024;
            FileEvent e2 = new FileEvent(); e2.EventId = "2"; e2.Size = 4 * 1024;
            Peer p1 = new Peer(); p1.Port = 1111;
            Peer p2 = new Peer(); p2.Port = 2222;
            Peer p3 = new Peer(); p3.Port = 3333;
            List<FileEvent> l1 = new List<FileEvent>(); l1.Add(e1);
            List<FileEvent> l2 = new List<FileEvent>(); l2.Add(e2);

            // Many user have l1
            manager.AddRequests(l1, p1);
            manager.AddRequests(l1, p2);

            // Few user have l2
            manager.AddRequests(l2, p3);

            // Should be l2
            var tlp = manager.GetRequests();
            //Assert.IsTrue(Enumerable.SequenceEqual(tlp.Item1, l2));
        }
    }
}
