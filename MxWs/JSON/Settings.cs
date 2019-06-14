//// MxWs
//// 3rd Gen MxW Server
//// BY MIKX
//// https://git.mikx.xyz/wow/MxWs
//// https://wow.mikx.xyz
//// Licensed under the Apache License 2.0

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MxWs.JSON
{
    public class SETTINGS
    {
        public string apikey { get; set; }
        public string apisecret { get; set; }
        public string dbname { get; set; }
        public string dbhost { get; set; }
        public string dbuser { get; set; }
        public string dbpass { get; set; }
        public int dump_day { get; set; }
        public int dump_hour { get; set; }
    }

    class Settings
    {
        internal static void ReadSettings()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            Server.settings_apiKey = j.apikey;
            Utilities.MSG.CMW(string.Format("APIKey: {0}", j.apikey), true, 1);
            Server.settings_apiSecret = j.apisecret;
            Utilities.MSG.CMW(string.Format("APISecret: {0}", j.apisecret), true, 1);
            Server.settings_dbname = j.dbname;
            Utilities.MSG.CMW(string.Format("DB Name: {0}", j.dbname), true, 1);
            Server.settings_dbhost = j.dbhost;
            Utilities.MSG.CMW(string.Format("DB Host: {0}", j.dbhost), true, 1);
            Server.settings_dbuser = j.dbuser;
            Utilities.MSG.CMW(string.Format("DB User: {0}", j.dbuser), true, 1);
            Server.settings_dbpass = j.dbpass;

            Server.settings_dumpd = j.dump_day;
            Server.settings_dumph = j.dump_hour;
        }
    }
}
