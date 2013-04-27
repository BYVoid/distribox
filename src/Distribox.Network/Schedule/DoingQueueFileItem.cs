using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Network
{
    using Distribox.FileSystem;

    struct DoingQueueFileItem
    {
        public FileEvent file;
        public List<Peer> whoHaveMe;
    }    
}
