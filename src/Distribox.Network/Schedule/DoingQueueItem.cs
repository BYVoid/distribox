using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Network
{
    public struct DoingQueueItem
    {
        public List<DoingQueueFileItem> files;
        public int filesHash;
        public DateTime expireDate;
        public int bandWidth;
    }
}
