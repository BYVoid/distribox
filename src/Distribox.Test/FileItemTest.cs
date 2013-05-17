using Distribox.FileSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Distribox.CommonLib;

namespace Distribox.Test
{
    [TestFixture]
    public class FileItemTest
    {
        [Test]
        public void TestOrder()
        {
            if (Directory.Exists(".Distribox"))
            {
                Directory.Delete(".Distribox", true);
            }

            Directory.CreateDirectory(".Distribox");
            Directory.CreateDirectory(".Distribox\\data");
            try
            {
                FileItem item = new FileItem();
                File.WriteAllText("Haha", "");
                item.Create("Haha", DateTime.FromFileTimeUtc(1));
                File.WriteAllText("Haha", "haha");
                File.Copy("Haha", ".Distribox\\data\\" + CommonHelper.GetSHA1Hash("Haha"));
                item.Change("Haha", CommonHelper.GetSHA1Hash("Haha"), DateTime.FromFileTimeUtc(2));
                Assert.AreEqual(2, item.History.Count());
            }
            finally
            {
                Directory.Delete(".Distribox", true);
            }
        }
    }
}
