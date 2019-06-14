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

    public class Index
    {
        public List<string> index { get; set; }
    }

    public class RootObject
    {
        public List<object> Misc { get; set; }
        public List<int> Tailoring { get; set; }
        public List<int> Skinning { get; set; }
        public List<int> Herbalism { get; set; }
        public List<int> Mining { get; set; }
        public List<int> Fishing { get; set; }
    }

    class IndexGen
    {
        internal static void GenIndex(string dumpid,string index)
        {
            string did = string.Format("dumpid='{0}'",dumpid);
            StringBuilder sb = new StringBuilder("INSERT INTO ah_index (dumpid,itemid,contentid,catid,value) VALUES ");
            List<string> Rows = new List<string>();
            List<string> ind = new List<string>();

            Index ji = JsonConvert.DeserializeObject<Index>(File.ReadAllText(index));

            foreach (var i in ji.index)
            {
                ind.Add(i);
            }

            string duid = Server.dumpID;
            //string duid = "nB3qJHOVPH";
            int cid = 1;
            int catid = 0;
            int itemid = 0;
            foreach (var e in ind)
            {              
                RootObject j = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText(string.Format("index_{0}.json",e)));
                if(j.Misc.Count > 0)
                {
                    foreach (var d in j.Misc)
                    {
                        catid = 1;
                        itemid = Convert.ToInt32(d);
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", duid, itemid, cid, catid, GetItemValue(itemid)));                        
                    }
                }

                if (j.Tailoring.Count > 0)
                {
                    foreach (var d in j.Tailoring)
                    {
                        catid = 2;
                        itemid = Convert.ToInt32(d);
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", duid, itemid, cid, catid, GetItemValue(itemid)));
                    }
                }

                if (j.Skinning.Count > 0)
                {
                    foreach (var d in j.Skinning)
                    {
                        catid = 3;
                        itemid = Convert.ToInt32(d);
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", duid, itemid, cid, catid, GetItemValue(itemid)));
                    }
                }

                if (j.Herbalism.Count > 0)
                {
                    foreach (var d in j.Herbalism)
                    {
                        catid = 4;
                        itemid = Convert.ToInt32(d);
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", duid, itemid, cid, catid, GetItemValue(itemid)));
                    }
                }

                if (j.Mining.Count > 0)
                {
                    foreach (var d in j.Mining)
                    {
                        catid = 5;
                        itemid = Convert.ToInt32(d);
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", duid, itemid, cid, catid, GetItemValue(itemid)));
                    }
                }

                if (j.Fishing.Count > 0)
                {
                    foreach (var d in j.Fishing)
                    {
                        catid = 6;
                        itemid = Convert.ToInt32(d);
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", duid, itemid, cid, catid, GetItemValue(itemid)));
                    }
                }                
                cid = cid + 1;
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

            string stm = string.Format("SELECT * FROM ah_items WHERE dumpid='{0}' AND itemid='{1}'",d,id);
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
