//// MxWs
//// 3rd Gen MxW Server
//// BY MIKX
//// https://git.mikx.xyz/wow/MxWs
//// https://wow.mikx.xyz
//// Licensed under the Apache License 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MxWs
{
    class Server
    {
        public static string version = "3.1.1";
        public static bool debug = false;
        public static bool serverLoop = true;
        public static bool indexBool = false;
        public static bool specialBool = false;

        public static string st = "init";
        public static string dumpID = "";
        public static string indexString = "";
        public static string specialString = "";

        // Settings Variables
        // Values are set by ReadSettings() during "init" state.
        public static string settings_apiKey = ""; // Store the Blizzard Dev API Key
        public static string settings_apiSecret = ""; // Store the Blizzard Dev API Secret
        public static string settings_dbname = ""; // Store the MySQL Database Name
        public static string settings_dbhost = ""; // Store the MySQL Database Host
        public static string settings_dbuser = ""; // Store the MySQL Database User
        public static string settings_dbpass = ""; // Store the MySQL Database Pass
        public static int settings_dumpd; // internal
        public static int settings_dumph; // internal
        public static string region = "";
        public static string realm = "";

        public static bool skipDump;

        static void Main(string[] args)
        {
            //// Main Server Loop
            while (serverLoop)
            {
                // Go to State Logic IF there is args and they contains region & realm OR debug = true
                if (args.Length > 0 && args.Contains("--region") && args.Contains("--realm"))
                {
                    // Get the region/realm from the args
                    if (args.Length > 0)
                    {
                        int regi = Array.IndexOf(args, "--region");
                        int reai = Array.IndexOf(args, "--realm");
                        region = args[regi + 1];
                        realm = args[reai + 1];
                    }  
                    // Index Arg Logic
                    if (args.Contains("--index"))
                    {
                        indexBool = true;
                        int ini = Array.IndexOf(args, "--index");
                        indexString = args[ini + 1];

                    }
                    // Special Arg Logic
                    if (args.Contains("--special"))
                    {
                        specialBool = true;
                        int ini = Array.IndexOf(args, "--special");
                        specialString = args[ini + 1];

                    }
                    // SkipDump Arg Logic
                    if (args.Contains("--skipdump"))
                    {
                        skipDump = true;
                    }
                    //// State Logic
                    switch (st)
                    {
                        // STATE: INIT (Read settings. Check environment.)
                        case "init":
                            Utilities.MSG.Splash();
                            Utilities.MSG.CMW("Reading settings...",true,1);
                            JSON.Settings.ReadSettings(); // Read the settings.                           
                            Utilities.MSG.CMW("Generating a DumpID...", true, 1);
                            dumpID = Utilities.Random.GenDumpID(); // Gen a new DumpID and save it to dumpID

                            Utilities.MSG.CMW(@"""init"" state done. Moving to ""get_dump"" state.", true, 2);
                            st = "get_dump";
                            //st = "gen_index";
                            break;

                        case "get_dump":
                            Utilities.MSG.CMW($"Last dump: D{settings_dumpd} H{settings_dumph} (D{DateTime.Now.Day} H{DateTime.Now.Hour})", true, 1);
                            if (settings_dumpd != DateTime.Now.Day && !skipDump)
                            {
                                if (settings_dumph != DateTime.Now.Hour)
                                {
                                    DumpLogic();
                                }
                            } else if (settings_dumpd == DateTime.Now.Day && !skipDump)
                            {
                                if (settings_dumph != DateTime.Now.Hour)
                                {
                                    DumpLogic();
                                }
                            }
                            st = "gen_aucdata";
                            break;

                        case "gen_aucdata":
                            Utilities.MSG.CMW("Preparing the auctions data...", true, 1);
                            JSON.AHDump.GenTable(); // Generate the internal table from the ah dump
                            Utilities.MSG.CMW("Generating the dump table entry...", true, 1);
                            Utilities.DB.GenDumpTable(); // Generate the "ah_dump" table data
                            Utilities.MSG.CMW("Generating the items table...", true, 1);
                            Utilities.DB.GenItemsTable(); // Generate the "ah_items" table data
                            Utilities.MSG.CMW("Generating the listing table...", true, 1);
                            Utilities.DB.GenListingTable(); // Generate the "ah_listing" table data                          

                            if (indexBool)
                            {
                                st = "gen_index";
                            } else
                            {
                                st = "clean";
                            }
                            break;
                        case "gen_index":
                            Utilities.MSG.CMW("Generating the index table...", true, 1);
                            JSON.IndexGen.GenIndex(dumpID, indexString);
                            if (specialBool)
                            {
                                st = "gen_special";
                            }
                            else
                            {
                                st = "clean";
                            }
                            break;
                        case "gen_special":
                            Utilities.MSG.CMW("Generating the special table...", true, 1);
                            JSON.SpecialGen.GenSpecial(dumpID, specialString);
                            st = "clean";
                            break;
                        case "clean":
                            Utilities.MSG.CMW("Cleaning the tables...", true, 1);
                            Utilities.DB.CleanDB(); // Clean the database, keeping the current day data.
                            Utilities.MSG.CMW(string.Format(@"DumpID ""{0}"" done!", dumpID), true, 1);
                            serverLoop = false;
                            break;
                    }
                } else
                {
                    // Print how to use the program and turn the loop off.
                    Utilities.MSG.Splash();
                    Utilities.MSG.CMW("USAGE: MxWs.exe --region <region> --realm <realm>", true, 3);
                    Utilities.MSG.CMW("Exemple: MxWs.exe us stormrage", true, 3);
                    Utilities.MSG.CMW("Option(s):", true, 3);
                    Utilities.MSG.CMW("[--index <file>] Used to generate the index content of my website.", true, 3);
                    Utilities.MSG.CMW("Press any key to quit...", true, 1);
                    Console.ReadKey();
                    serverLoop = false;
                }              
            }
        }

        private static void DumpLogic()
        {
            Utilities.MSG.CMW("Requesting latest json url from BNEt...", true, 1);
            string jsonurl = JSON.AHUrl.GetURL(region, realm, settings_apiKey); // Get the json url from BNET
            Utilities.MSG.CMW(string.Format("URL: {0}", jsonurl), true, 2);
            Utilities.MSG.CMW("Downloading the latest AH dump from BNet...", true, 1);
            Utilities.Web.DownloadFileNoAsync(jsonurl, string.Format("{0}_{1}-dump.json", region, realm)); // Download the latest ah dump using the obtained url
            Utilities.MSG.CMW(@"""get_dump"" state done. Moving to ""gen_aucdata"" state.", true, 2);
            settings_dumpd = DateTime.Now.Day;
            settings_dumph = DateTime.Now.Hour;
        }
    }
}
