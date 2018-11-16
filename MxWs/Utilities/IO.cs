//// MxWs
//// 3rd Gen MxW Server
//// BY MIKX
//// https://git.mikx.xyz/wow/MxWs
//// https://wow.mikx.xyz
//// Licensed under the Apache License 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MxWs.Utilities
{
    class IO
    {
        public static void CheckDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Utilities.MSG.CMW(string.Format("[IO] Directory {0} not found. Creating...",path), true, 3);
                Directory.CreateDirectory(path);
            }
        }
    }
}
