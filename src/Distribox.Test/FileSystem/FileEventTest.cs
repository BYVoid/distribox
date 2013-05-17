using Distribox.FileSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Test
{
    [TestFixture]
    public class FileEventTest
    {
        [Test]
        public void Test()
        {
            FileEvent e1 = FileEvent.CreateEvent();
            FileEvent e2 = FileEvent.CreateEvent();

            // Test hash code
            Assert.AreNotEqual(e1.GetHashCode(), e2.GetHashCode());

            e2.EventId = e1.EventId;
            Assert.IsTrue(e1.Equals(e2));
        }
    }
}
