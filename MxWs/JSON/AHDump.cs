//// MxWs
//// 3rd Gen MxW Server
//// BY MIKX
//// https://git.mikx.xyz/wow/MxWs
//// https://wow.mikx.xyz
//// Licensed under the Apache License 2.0

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MxWs.JSON
{
    class AHDump
    {
        public class Realm
        {
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Auction
        {
            public Int64 auc { get; set; }
            public Int64 item { get; set; }
            public string owner { get; set; }
            public string ownerRealm { get; set; }
            public Int64 bid { get; set; }
            public Int64 buyout { get; set; }
            public int quantity { get; set; }
            public string timeLeft { get; set; }
            public int rand { get; set; }
            public Int64 seed { get; set; }
            public int context { get; set; }
        }

        public class UID
        {
            public Int64 auc { get; set; }
            public Int64 item { get; set; }
            public Int64 buyout { get; set; }
        }

        public class RootObject
        {
            public List<Realm> realms { get; set; }
            public List<Auction> auctions { get; set; }
            public List<UID> uid { get; set; }
        }

        public static DataSet aucds = new DataSet();

        public static void GenTable()
        {
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText(string.Format("{0}_{1}-dump.json", Server.region, Server.realm)));
            DataTable table = ds.Tables["auctions"];

            DataTable auctb = new DataTable("auclist");

            auctb.Columns.Add(new DataColumn("auc", typeof(Int64)));
            auctb.Columns.Add(new DataColumn("item", typeof(Int64)));
            auctb.Columns.Add(new DataColumn("owner", typeof(string)));
            auctb.Columns.Add(new DataColumn("value", typeof(Int64)));
            auctb.Columns.Add(new DataColumn("qty", typeof(int)));
            auctb.Columns.Add(new DataColumn("time", typeof(string)));

            foreach (DataRow dr in table.Rows)
            {               
                Int64 buy = Convert.ToInt64(dr["buyout"]);

                if (buy > 0)
                {
                    DataRow nr = auctb.NewRow();

                    nr["auc"] = Convert.ToInt32(dr["auc"]);
                    nr["item"] = Convert.ToInt32(dr["item"]);
                    nr["owner"] = dr["owner"];
                    nr["qty"] = Convert.ToInt32(dr["quantity"]);

                    int qty = Convert.ToInt32(dr["quantity"]);

                    if (qty == 1)
                    {
                        nr["value"] = buy;
                    }
                    else
                    {
                        nr["value"] = buy / qty;
                    }

                    auctb.Rows.Add(nr);
                }               
            }
            aucds.Tables.Add(auctb);
        }
    }
}
