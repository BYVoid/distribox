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
        public FileItemTest()
        {
            if (!Directory.Exists(".Distribox"))
            {
                Directory.CreateDirectory(".Distribox");
            }
            if (!Directory.Exists(".Distribox/data"))
            {
                Directory.CreateDirectory(".Distribox/data");
            }
            if (Directory.Exists("dir"))
            {
                Directory.Delete("dir");
            }
            if (Directory.Exists("dir2"))
            {
                Directory.Delete("dir2");
            }
            File.WriteAllText(".Distribox/data/11", "haha");
        }

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

        [Test]
        public void MergeTest()
        {
            // File Test
            FileItem file = new FileItem();

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Created;
                e.Name = "haha";
                e.SHA1 = "11";
                e.When = DateTime.FromFileTimeUtc(1);
                file.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Created;
                e.Name = "haha";
                e.SHA1 = "11";
                e.When = DateTime.FromFileTimeUtc(2);
                file.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Created;
                e.Name = "haha";
                e.SHA1 = null;
                e.When = DateTime.FromFileTimeUtc(3);
                file.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Renamed;
                e.Name = "haha";
                e.SHA1 = null;
                e.When = DateTime.FromFileTimeUtc(4);
                file.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Changed;
                e.Name = "haha";
                e.SHA1 = "11";
                e.When = DateTime.FromFileTimeUtc(5);
                file.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Deleted;
                e.Name = "haha";
                e.SHA1 = "11";
                e.When = DateTime.FromFileTimeUtc(6);
                file.Merge(e);
            }

            FileItem dir = new FileItem();

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Created;
                e.IsDirectory = true;
                e.Name = "dir";
                e.When = DateTime.FromFileTimeUtc(1);
                dir.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Created;
                e.IsDirectory = true;
                e.Name = "dir";
                e.When = DateTime.FromFileTimeUtc(2);
                dir.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Created;
                e.IsDirectory = true;
                e.Name = "dir";
                e.When = DateTime.FromFileTimeUtc(3);
                dir.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Changed;
                e.IsDirectory = true;
                e.Name = "dir";
                e.When = DateTime.FromFileTimeUtc(4);
                dir.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Renamed;
                e.IsDirectory = true;
                e.Name = "dir2";
                e.When = DateTime.FromFileTimeUtc(5);
                dir.Merge(e);
            }

            {
                FileEvent e = new FileEvent();
                e.Type = FileEventType.Deleted;
                e.IsDirectory = true;
                e.Name = "dir2";
                e.When = DateTime.FromFileTimeUtc(6);
                dir.Merge(e);
            }
        }
    }
}
