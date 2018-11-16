//// MxWs
//// 3rd Gen MxW Server
//// BY MIKX
//// https://git.mikx.xyz/wow/MxWs
//// https://wow.mikx.xyz
//// Licensed under the Apache License 2.0

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MxWs.Utilities
{
    class DB
    {
        public static MySqlConnection ItemDB()
        {
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};",Server.settings_dbhost, Server.settings_dbuser,Server.settings_dbpass,Server.settings_dbname);
            conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;
            return conn;
        }

        public static void GenDumpTable()
        {
            string d = Server.dumpID;
            int ac = JSON.AHDump.aucds.Tables[0].Select("item is not null").Length;

            MySqlConnection mc = ItemDB();
            MySqlCommand command = mc.CreateCommand();
            command.CommandText = string.Format("INSERT INTO ah_dump (dumpid,region,realm,year,month,day,hour,quantity) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", d, Server.region, Server.realm, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, ac);
            mc.Open();
            command.ExecuteNonQuery();
            mc.Close();
        }

        public static void GenItemsTable()
        {
            string d = Server.dumpID;

            string stm = "SELECT * FROM items";
            MySqlConnection mc = ItemDB();
            MySqlCommand cmd = new MySqlCommand(stm, mc);
            mc.Open();

            DataTable data = new DataTable();
            data.Load(cmd.ExecuteReader());

            using (MySqlConnection mcc = ItemDB())
            {
                StringBuilder sCommand = new StringBuilder("INSERT INTO ah_items (dumpid,itemid,maximum,median,minimum,quantity) VALUES ");
                List<string> Rows = new List<string>();
                DataRow[] result = JSON.AHDump.aucds.Tables["auclist"].Select("item is not null");
                foreach (DataRow row in data.Rows)
                {
                    int i = Convert.ToInt32(row["id"]);
                    int ie = JSON.AHDump.aucds.Tables[0].Select(string.Format("item = '{0}'", i)).Length;
                    if (ie >= 1)
                    {
                        Int64 median = Convert.ToInt64(JSON.AHDump.aucds.Tables["auclist"].Compute("AVG(value)", string.Format("item = '{0}'", i)));
                        Int64 maximum = Convert.ToInt64(JSON.AHDump.aucds.Tables["auclist"].Compute("MAX(value)", string.Format("item = '{0}'", i)));
                        Int64 minimum = Convert.ToInt64(JSON.AHDump.aucds.Tables["auclist"].Compute("MIN(value)", string.Format("item = '{0}'", i)));
                        int qty = JSON.AHDump.aucds.Tables["auclist"].Select(string.Format("item = '{0}'", i)).Length;
                        Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}')", d, i, maximum, median, minimum, qty));
                    }
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                mcc.Open();
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mcc))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
        }

        public static void GenListingTable()
        {
            string d = Server.dumpID;

            using (MySqlConnection mcc = ItemDB())
            {
                StringBuilder sCommand = new StringBuilder("INSERT INTO ah_listing (dumpid,itemid,owner,quantity,uvalue,tvalue) VALUES ");
                List<string> Rows = new List<string>();
                DataRow[] result = JSON.AHDump.aucds.Tables["auclist"].Select("item is not null");
                foreach (DataRow alr in result)
                {
                    int i = Convert.ToInt32(alr["item"]);
                    string owner = alr["owner"].ToString();
                    int quantity = Convert.ToInt32(alr["qty"]);
                    Int64 uvalue = Convert.ToInt64(alr["value"]);
                    Int64 tvalue = Convert.ToInt64(alr["value"]) * Convert.ToInt64(alr["qty"]);
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}')", d, i, owner, quantity, uvalue, tvalue));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                mcc.Open();
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mcc))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
        }

        public static void CleanDB()
        {
            int day = DateTime.Now.Day;

            string stm = string.Format("SELECT * FROM ah_dump WHERE day <> {0}", day);
            MySqlConnection mc = ItemDB();
            MySqlCommand cmd = new MySqlCommand(stm, mc);
            mc.Open();

            MySqlDataReader result = cmd.ExecuteReader();
            
            while (result.Read())
            {
                string did = result["dumpid"].ToString();

                string stm2 = string.Format("DELETE FROM ah_items WHERE dumpid='{0}'", did);
                MySqlConnection mc2 = ItemDB();
                MySqlCommand cmd2 = new MySqlCommand(stm2, mc2);
                mc2.Open();
                cmd2.ExecuteNonQuery();
                mc2.Close();

                string stm3 = string.Format("DELETE FROM ah_listing WHERE dumpid='{0}'", did);
                MySqlConnection mc3 = ItemDB();
                MySqlCommand cmd3 = new MySqlCommand(stm3, mc3);
                mc3.Open();
                cmd3.ExecuteNonQuery();
                mc3.Close();

                string stm4 = string.Format("DELETE FROM ah_dump WHERE dumpid='{0}'", did);
                MySqlConnection mc4 = ItemDB();
                MySqlCommand cmd4 = new MySqlCommand(stm4, mc4);
                mc4.Open();
                cmd4.ExecuteNonQuery();
                mc4.Close();

                string stm5 = string.Format("DELETE FROM ah_index WHERE dumpid='{0}'", did);
                MySqlConnection mc5 = ItemDB();
                MySqlCommand cmd5 = new MySqlCommand(stm5, mc5);
                mc5.Open();
                cmd5.ExecuteNonQuery();
                mc5.Close();
            }
        }
    }
}
