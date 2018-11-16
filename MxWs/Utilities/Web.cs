//// MxWs
//// 3rd Gen MxW Server
//// BY MIKX
//// https://git.mikx.xyz/wow/MxWs
//// https://wow.mikx.xyz
//// Licensed under the Apache License 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MxWs.Utilities
{
    class Web
    {
        public static void DownloadFileNoAsync(string url, string file)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(url, file);
            }
        }
    }
}
