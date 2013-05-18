//-----------------------------------------------------------------------
// <copyright file="GlobalFlagTest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Test
{    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Distribox.CommonLib;
    using NUnit.Framework;

    /// <summary>
    /// Test entry for global flag.
    /// </summary>
    [TestFixture]
    public class GlobalFlagTest
    {
        /// <summary>
        /// Test global flag.
        /// </summary>
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
