//// MxWs
//// 3rd Gen MxW Server
//// BY MIKX
//// https://git.mikx.xyz/wow/MxWs
//// https://wow.mikx.xyz
//// Licensed under the Apache License 2.0

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MxWs.JSON
{
    class AHUrl
    {
        public class JSON
        {
            public string url { get; set; }
            public long lastModified { get; set; }
        }

        public class RootObject
        {
            public List<JSON> files { get; set; }
        }

        public static string GetURL(string region, string realm, string api)
        {
            string token = Utilities.OAuth.GetOAuth(Server.settings_apiKey, Server.settings_apiSecret);
            using (WebClient wc = new WebClient())
            {
                ServicePointManager.ServerCertificateValidationCallback += (p1, p2, p3, p4) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var json = wc.DownloadString($@"https://{Server.region}.api.blizzard.com/wow/auction/data/{Server.realm}?locale=en_US&access_token={token}");
                RootObject j = JsonConvert.DeserializeObject<RootObject>(json);
                return j.files[0].url;
            }
        }
    }
}
