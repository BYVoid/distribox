using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Network.Schedule
{
    using System.Threading;
    using Distribox.CommonLib;
    using Distribox.FileSystem;
    using NUnit.Framework;

    [TestFixture]
    class RequestManagerTest
    {
        [Test]
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
            Thread.Sleep(5000);

            // Should be available again
            tlp = manager.GetRequests();
            Assert.IsNotNull(tlp);
            Assert.IsTrue(Enumerable.SequenceEqual(tlp.Item1, l1));
            Assert.AreEqual(tlp.Item2, p1);

            // No request
            Assert.IsNull(manager.GetRequests());

            // The request finished
            manager.FinishRequests(l1);

            // No request
            Assert.IsNull(manager.GetRequests());
        }

        [Test]
        public void SpeedHeuristics()
        {

        }

        [Test]
        public void UniquenessHeuristics()
        {

        }
    }
}
