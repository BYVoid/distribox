using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class GlobalFlag
    {
        public static bool AcceptFileEvent { get; set; }

        static GlobalFlag()
        {
            AcceptFileEvent = true;
        }
    }
}
