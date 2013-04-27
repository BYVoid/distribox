using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.Network
{
    using Distribox.FileSystem;

    struct UniquenessFileEventPair : IComparable<UniquenessFileEventPair>
    {
        public double uniqueness;
        public FileEvent fileEvent;

        public int CompareTo(UniquenessFileEventPair other)
        {
            if (uniqueness > other.uniqueness)
            {
                return -1;
            }

            if (uniqueness == other.uniqueness)
            {
                return 0;
            }

            return 1;
        }
    }
}
