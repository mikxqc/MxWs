using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MxWs.JSON
{
    public class Special
    {
        public List<string> special { get; set; }
    }

    public class SpecialObject
    {
        public List<object> Data { get; set; }
    }

    class SpecialGen
    {
        internal static void GenSpecial(string dumpid, string special)
        {
            string did = string.Format("dumpid='{0}'", dumpid);
            StringBuilder sb = new StringBuilder("INSERT INTO ah_special_data (dumpid,itemid,specialid,value) VALUES ");
            List<string> Rows = new List<string>();
            List<string> ind = new List<string>();

            Special ji = JsonConvert.DeserializeObject<Special>(File.ReadAllText(special));

            foreach (var i in ji.special)
            {
                ind.Add(i);
            }

            string duid = Server.dumpID;
            int sid = 1;
            int itemid = 0;
            foreach (var e in ind)
            {
                SpecialObject j = JsonConvert.DeserializeObject<SpecialObject>(File.ReadAllText(string.Format("json/special_{0}.json", e)));

                if (j.Data.Count > 0)
                {
                    foreach (var d in j.Data)
                    {
                        itemid = Convert.ToInt32(d);
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}')", duid, itemid, sid, GetItemValue(itemid)));
                    }
                }
                sid = sid + 1;
            }
            sb.Append(string.Join(",", Rows));
            sb.Append(";");
            InsertIndex(sb.ToString());
        }

        public static int GetItemValue(int id)
        {
            string d = Server.dumpID;
            //string d = "nB3qJHOVPH";
            int v = 0;

            string stm = string.Format("SELECT * FROM ah_items WHERE dumpid='{0}' AND itemid='{1}'", d, id);
            MySqlConnection mc = Utilities.DB.ItemDB();
            MySqlCommand cmd = new MySqlCommand(stm, mc);
            mc.Open();

            MySqlDataReader result = cmd.ExecuteReader();

            if (result.Read())
            {
                v = Convert.ToInt32(result["minimum"]);
            }

            mc.Close();
            return v;
        }

        private static void InsertIndex(string cmd)
        {
            MySqlConnection mc = Utilities.DB.ItemDB();
            MySqlCommand command = mc.CreateCommand();
            command.CommandText = cmd;
            mc.Open();
            command.ExecuteNonQuery();
            mc.Close();
        }
    }
}
