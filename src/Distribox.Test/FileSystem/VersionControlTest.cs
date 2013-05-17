using Distribox.FileSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Distribox.Test
{
    [TestFixture]
    public class VersionControlTest
    {
        private List<FileItem> list = new List<FileItem>();

        public VersionControlTest()
        {
            if (!Directory.Exists(".Distribox"))
            {
                Directory.CreateDirectory(".Distribox");
            }
            if (!Directory.Exists(".Distribox/data"))
            {
                Directory.CreateDirectory(".Distribox/data");
            }
            if (Directory.Exists("Dir"))
            {
                Directory.Delete("Dir");
            }

            FileEvent e1 = new FileEvent();
            e1.Type = FileEventType.Created;
            e1.EventId = "1";
            e1.FileId = "1";
            e1.Name = "haha";

            FileEvent e2 = new FileEvent();
            e2.Type = FileEventType.Changed;
            e2.EventId = "2";
            e2.FileId = "1";
            e2.SHA1 = "11";
            e2.Name = "haha";
            File.WriteAllText(".Distribox/data/11", "haha");

            FileEvent e3 = new FileEvent();
            e3.Type = FileEventType.Renamed;
            e3.EventId = "3";
            e3.FileId = "1";
            e3.SHA1 = "11";
            e3.Name = "XX";

            FileEvent e4 = new FileEvent();
            e4.Type = FileEventType.Deleted;
            e4.EventId = "4";
            e4.FileId = "1";
            e4.Name = "XX";

            FileEvent e5 = new FileEvent();
            e5.Type = FileEventType.Created;
            e5.EventId = "5";
            e5.FileId = "2";
            e5.Name = "TT";

            FileEvent e6 = new FileEvent();
            e6.Type = FileEventType.Changed;
            e6.EventId = "6";
            e6.FileId = "2";
            e6.SHA1 = "11";
            e6.Name = "TT";

            FileEvent e7 = new FileEvent();
            e7.Type = FileEventType.Created;
            e7.EventId = "7";
            e7.FileId = "3";
            e7.Name = "Dir";
            e7.IsDirectory = true;
            e7.When = DateTime.FromFileTimeUtc(7);

            FileItem item1 = new FileItem();
            item1.Merge(e1);
            item1.Merge(e2);
            item1.Merge(e3);
            item1.Merge(e4);

            FileItem item2 = new FileItem();
            item2.Merge(e5);
            item2.Merge(e6);

            FileItem item3 = new FileItem();
            item3.Merge(e7);

            this.list.Add(item1);
            this.list.Add(item2);
            this.list.Add(item3);
        }

        [Test]
        public void GetLessThanTest()
        {
            var vl1 = new VersionList();
            vl1.AllFiles = this.list;

            var vl2 = new VersionList();
            vl2.GetLessThan(vl1);
        }

        [Test]
        public void Test()
        {
            byte[] bytes = VersionControl.CreateFileBundle(this.list.First().History);

            var vc = new VersionControl();

            List<FileEvent> receive = vc.AcceptFileBundle(bytes);
        }

        [Test]
        public void GetFileByNameTest()
        {
            var vc = new VersionControl();
            byte[] bytes = VersionControl.CreateFileBundle(this.list.SelectMany(x => x.History).ToList());
            List<FileEvent> receive = vc.AcceptFileBundle(bytes);
            vc.VersionList.GetFileByName("TT");
            vc.VersionList.GetFileById("2");
            vc.VersionList.RemoveFileByName("TT");
        }

        [Test]
        public void CheckOutTest()
        {
            var vc = new VersionControl();
            byte[] bytes = VersionControl.CreateFileBundle(this.list.SelectMany(x => x.History).ToList());
            List<FileEvent> receive = vc.AcceptFileBundle(bytes);
            vc.CheckOut("2", "5");
        }

    }
}
