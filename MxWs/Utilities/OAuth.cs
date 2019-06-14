using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MxWs.Utilities
{
    class OAuth
    {
        public class OAuthObjects
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }

        public static string GetOAuth(string key, string secret)
        {           
            using (WebClient wc = new WebClient())
            {
                ServicePointManager.ServerCertificateValidationCallback += (p1, p2, p3, p4) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var json = wc.DownloadString($@"https://us.battle.net/oauth/token?client_id={key}&client_secret={secret}&grant_type=client_credentials");
                OAuthObjects j = JsonConvert.DeserializeObject<OAuthObjects>(json);
                return j.access_token;
            }
        }     
    }
}
