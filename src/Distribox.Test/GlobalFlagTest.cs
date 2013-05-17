using Distribox.CommonLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Test
{
    [TestFixture]
    class GlobalFlagTest
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(true, GlobalFlag.AcceptFileEvent);

            GlobalFlag.AcceptFileEvent = false;
            Assert.AreEqual(false, GlobalFlag.AcceptFileEvent);

            GlobalFlag.AcceptFileEvent = true;
            Assert.AreEqual(true, GlobalFlag.AcceptFileEvent);
        }
    }
}
