// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="CesarVelez">
//   Cesar Velez - Copyright 2013
// </copyright>
// <summary>
//   Driver Program for AprioriMiner
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AprioriMiner
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;

    using AprioriMiner.Models;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using MySql.Data.MySqlClient;

    /// <summary>
    /// Driver Program for AprioriMiner
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry Point To Program
        /// </summary>
        /// <param name="args">command line arguments (not used)</param>
        [STAThread]
        public static void Main(string[] args)
        {

            Console.WriteLine("===================================");
            Console.WriteLine("  Welcome to AprioriMiner v1.0");
            Console.WriteLine("===================================");
            Console.WriteLine(
                "To run this program correctly the DataTransformer program must have been run correctly\nIf so, proceed...");


            var showMenu = true;

            while (showMenu)
            {
                showMenu = ShowMainMenu();
            }

            // end of program
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();

        }

        /// <summary>
        /// Prompts user if they want to continue
        /// </summary>
        /// <returns>boolean result of user choice</returns>
        private static bool PromptToContinue()
        {
            bool output;

            while (true)
            {
                Console.Write("\n\n\tWould you like to continue? Enter Y or N: ");
                var ans = Convert.ToString(Console.ReadKey().KeyChar);
                if (ans.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    output = true;
                    break;
                }
                if (ans.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    output = false;
                    break;
                }
                Console.WriteLine("\nSorry did not catch that, try again, just enter 'Y' or 'N' for yes or no.");
            }
            return output;
        }

        /// <summary>
        /// Shows main Menu of options and returns true if user intends to continue
        /// </summary>
        /// <returns>Boolean indicates whether user wants to continue</returns>
        private static bool ShowMainMenu()
        {
            Console.WriteLine("\n***************************************************************");
            Console.WriteLine("\n\tMAIN MENU OPTIONS: ");
            Console.WriteLine();
            Console.WriteLine("\ta)\tApriori mine MongoDB     ");
            Console.WriteLine("\tb)\tApriori mine from mysql        ");
            Console.WriteLine("\tc)\tHelp/Info/About       ");
            Console.WriteLine("\tQ)\tQuit                                     ");
            Console.WriteLine("\n***************************************************************");

            Console.Write("Please enter the option of your choice from above: ");
            var ans = Convert.ToString(Console.ReadKey().KeyChar);
            Console.WriteLine();
            var cont = true;
            switch (ans)
            {
                case "a":
                case "A":
                    DataFromMongo();
                    cont = PromptToContinue();
                    break;
                case "b":
                case "B":
                    DataFromMysql();
                    cont = PromptToContinue();
                    break;
                case "c":
                case "C":
                    DisplayHelpInfo();
                    cont = PromptToContinue();
                    break;
                case "q":
                case "Q":
                    Console.WriteLine("\nQuitting...");
                    cont = false;
                    break;
                default:
                    Console.WriteLine("\n\nYour entry was not valid, please try again.");
                    break;
            }
            return cont;
        }

        /// <summary>
        /// Displays program info and help content
        /// </summary>
        private static void DisplayHelpInfo()
        {
            Console.Clear();
            Console.WriteLine("=============================================\n\tHelp/Info Menu\n=============================================\n");
            Console.WriteLine(
                "For this program to be able to mine using Apriori correctly, the following must be true:");
            Console.WriteLine(
                "\t1) A MongoDB server with a db called 'learn' must be running on default localhost 27017 port. \n'receipts' is the collection name.\n");
            Console.WriteLine(
                "\t2) Transaction Data in csv format located in the '\\data\\' folder should be inserted into tha 'receipts' collection via the DataTransformer program.\n");
            
            Console.WriteLine("\t***NOTE:MongoDB is embedded with this app and should run automatically via the RunAprioriMiner.bat commands.\nMake sure you allow mongod.exe to connect thru private (local) network if Windows Firewall prompts you. It uses the localhost(127.0.0.1) to get connections locally.");
            Console.WriteLine(
                "For MySQL, (ver. 5.6) a local server named 'mysql56' with a database called 'dataminingdb' which has an 'itemstbl', and 'itemsets' tables.");
            Console.WriteLine("Connect to that server using username: dmuser, and pwd='data'.");

            Console.WriteLine("If you have any questions on how to run this please email me at cvelez2@student.gsu.edu - thanks.");

        }

        private static void DataFromMongo()
        {
            double[] minsupminconf = PromptForMinsupAndConf();
            Console.WriteLine("\nConnecting to mongoDB...");

            var database = new ItemsetCollection();
            try
            {
                const string ConnStr = "mongodb://localhost:27017";
                var clt = new MongoClient(ConnStr);
                var svr = clt.GetServer();
                var dblearn = svr.GetDatabase("learn");

                // var tlist = dblearn.GetCollection("receipts");

                var list = dblearn.GetCollection<Transaction>("receipts");

                var all = list.FindAll();
                
                foreach (var transaction in all)
                {
                    var itemset = new Itemset();
                    foreach (var item in transaction.Items)
                    {
                        itemset.Add(item);
                    }

                    database.Add(itemset);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error, here are the details: \n=====================Error Stuff Here=====================\n" + e.Message + "\n==========================================================");
                Console.WriteLine("Is the mongodb server running?? See the Help/Info section for help...");
                return;
            }

            var itemsunique = CreateSetOfUniqueItems();

            Console.WriteLine("Now running Apriori on fetched data...");

            var large = AprioriMining.DoApriori(database, itemsunique, minsupminconf[0]);

            var results = "Results: \n\n " + large.Count + " supported Itemsets obtained by Apriori\n\n";
            foreach (var itemset in large)
            {
                results += itemset.ToString() + "\n";
            }

            Console.WriteLine("DONE! Now mining association rules...");
            var allRules = AprioriMining.Mine(database, large, minsupminconf[1]);

            results += "\nAssociation Rules Found: \n";
            if (allRules.Count == 0)
            {
                results += "No rules were found over minconf of " + minsupminconf[1] + "%";
            }
            else
            {
                foreach (var associationRule in allRules)
                {
                    results += associationRule.ToString() + "\n";
                }
            }

            Console.WriteLine(results);

        }

        /// <summary>
        /// Helper method for Apriori algorithm which just creates our itemset
        /// </summary>
        /// <returns></returns>
        private static Itemset CreateSetOfUniqueItems()
        {
            var itemsunique = new Itemset();
            var i = 0;
            while (i < 50)
            {
                itemsunique.Add(i);
                i++;
            }
            return itemsunique;
        }

        /// <summary>
        /// Prompts user for minsup and minconf percetages
        /// </summary>
        /// <returns>A double array where arr[0] is minsup and arr[1] is minconf</returns>
        private static double[] PromptForMinsupAndConf()
        {
            var ans = new double[2];
            var tries = 3;
            while (tries > 0)
            {
                try
                {
                    Console.Write("\n\n\tPlease enter the minimum support % (minsup) (ex: 60.0 or 60): ");
                    var minsup = Convert.ToDouble(Console.ReadLine());
                    ans[0] = minsup;
                    tries = 3;
                    break;
                }
                catch (Exception e)
                {
                    Console.Write("You either did not enter a number or your number was too big! Try again...");
                    tries--;
                }
            }
            while (tries > 0)
            {
                try
                {
                    Console.Write("\n\n\tPlease enter the min. confidence % (minconf) (ex: 70.0 or 75): ");
                    var minconf = Convert.ToDouble(Console.ReadLine());
                    ans[1] = minconf;
                    tries++;
                    break;
                }
                catch (Exception e)
                {
                    Console.Write(
                        "You either did not enter a number correctly or your number was too big! Try again...");
                    tries--;
                }
            }

            return ans;
        }

        /// <summary>
        /// Connects to MySQL in order to run AprioriMiner
        /// </summary>
        private static void DataFromMysql()
        {
            double[] minsupminconf = PromptForMinsupAndConf();
            
            Console.WriteLine("Connecting to mysql server....");
            var database = new ItemsetCollection();

            using (var conn = new MySqlConnection("server=localhost;user=dmuser;database=DataMiningDb;port=3306;password=data;"))
            {
                try
                {
                    conn.Open();
                    var rows = 0;

                    rows = CountTransactions(conn);

                    var cmd = conn.CreateCommand();

                    for (var i = 1; i <= rows; i++)
                    {
                        
                        cmd.CommandText = i == 1
                                              ? (@"set profiling=1; SELECT itemID FROM dataminingdb.itemsets WHERE transId="
                                                 + i)
                                              : @"SELECT itemID FROM dataminingdb.itemsets WHERE transId=" + i;

                        var reader = cmd.ExecuteReader();

                        var listofItemsInRow = new Itemset();

                        while (reader.Read())
                        {
                            listofItemsInRow.Add(reader.GetInt32(0));
                        }

                        database.Add(listofItemsInRow);
                        reader.Close();
                        // ============== LOG PROFILER DATA
                        cmd.CommandText = "SHOW PROFILES";
                        var pfdata = cmd.ExecuteReader();
                        var rs = 0;
                        
                        var list = new List<object[]>();
                        while (pfdata.Read())
                        {
                            var rowArray = new object[pfdata.FieldCount];
                            rs = pfdata.GetValues(rowArray);
                            list.Add(rowArray);
                        }
                        using (var swrtr = new StreamWriter(@"..\..\..\MysqlProfilerLog.csv", true))
                        {
                            // swrtr.WriteLine(DateTime.Now + " =  LOG  Success = {0}--------------------\n", rs);
                            // Query Number,Time,SQLStatement
                            foreach (var stringse in list)
                            {
                                if (stringse[2].Equals("SHOW WARNINGS"))
                                {
                                    continue;
                                }

                                foreach (var s in stringse)
                                {
                                    swrtr.Write(s + ",");
                                    if (stringse.ElementAt(2) == s)
                                    {
                                        swrtr.Write("\n");
                                    }
                                }
                                // swrtr.WriteLine(Environment.NewLine);
                            }

                        }

                        // ==============

                        pfdata.Close();
                        // Writing profiler info to log file
                        // RecordLog(conn);
                    } // End of For Loop

                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error: {0}\n\n Consult the help menu.", e.Message);
                }
            }

            // database (of ItemsetCollection should now be populated and ready for mining...

                Console.WriteLine("Now running Apriori on fetched data...");

                var large = AprioriMining.DoApriori(database, CreateSetOfUniqueItems(), minsupminconf[0]);

                var results = "Results: \n\n " + large.Count + " supported Itemsets obtained by Apriori\n\n";
                foreach (var itemset in large)
                {
                    results += itemset.ToString() + "\n";
                }

                Console.WriteLine("DONE! Now mining association rules...");
                var allRules = AprioriMining.Mine(database, large, minsupminconf[1]);

                results += "\nAssociation Rules Found: \n";
                if (allRules.Count == 0)
                {
                    results += "No rules were found over minconf of " + minsupminconf[1] + "%";
                }
                else
                {
                    foreach (var associationRule in allRules)
                    {
                        results += associationRule.ToString() + "\n";
                    }
                }

                Console.WriteLine(results);
        }

        /// <summary>
        /// Uses MySQL profiler and appends to log file.
        /// </summary>
        /// <param name="conn">The connection to database</param>
        private static void RecordLog(MySqlConnection conn)
        {
                    var profileCmd = conn.CreateCommand();
                    profileCmd.CommandText = "SHOW PROFILES";
                    var pfdata = profileCmd.ExecuteReader();
                    var rs = 0;
                    var rowArray = new object[pfdata.FieldCount];
                    var list = new List<object[]>();
                    while (pfdata.Read())
                    {
                        rs = pfdata.GetValues(rowArray);
                        list.Add(rowArray);
                    }
                    using (var swrtr = new StreamWriter(@"..\..\..\MysqlProfilerLog.txt", true))
                    {
                        // swrtr.WriteLine(DateTime.Now + " =  LOG  Success = {0}--------------------\n", rs);
                        foreach (var stringse in list)
                        {
                            foreach (var s in stringse)
                            {
                                swrtr.Write(s + ", ");
                            }
                            swrtr.WriteLine(Environment.NewLine);
                        }

                    }
        }

        private static int CountTransactions(MySqlConnection conn)
        {
            
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT transID, COUNT( DISTINCT transID) FROM dataminingdb.itemsets";
            var data = cmd.ExecuteReader();
            if (data.Read())
            {
                var numTrans = data.GetInt32(1);
                Console.WriteLine("Found {0} transactions...\n", numTrans);
                data.Close();
                return numTrans;
            }

            throw new Exception("There was an error...");
        }
    }
}
