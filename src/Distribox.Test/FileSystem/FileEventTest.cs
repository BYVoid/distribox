//-----------------------------------------------------------------------
// <copyright file="FileEventTest.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.Test
{    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Distribox.FileSystem;
    using NUnit.Framework;

    /// <summary>
    /// The Test class of FileEvent.
    /// </summary>
    [TestFixture]
    public class FileEventTest
    {        
        /// <summary>
        /// The main test entry.
        /// </summary>
        [Test]
        public void Test()
        {
            // Test create method.
            FileEvent e1 = FileEvent.CreateEvent();
            FileEvent e2 = FileEvent.CreateEvent();

            // Test hash code.
            Assert.AreNotEqual(e1.GetHashCode(), e2.GetHashCode());

            e2.EventId = e1.EventId;
            Assert.IsTrue(e1.Equals(e2));
        }
    }
}
