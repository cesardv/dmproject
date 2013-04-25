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
           //  Console.WriteLine("\tc)\t       ");
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
                //case "C":
                //    LittleTest();
                //    cont = PromptToContinue();
                //    break;
                case "q":
                case "Q":
                    Console.WriteLine("\nQuitting...");
                    cont = false;
                    break;
                default: Console.WriteLine("\n\nYour entry was not valid, please try again.");
                    break;
            }
            return cont;
        }

        private static void DataFromMongo()
        {
            double[] minsupminconf = PromptForMinsupAndConf();
            Console.WriteLine("\nConnecting to mongoDB...");

            const string ConnStr = "mongodb://localhost:27017";
            var clt = new MongoClient(ConnStr);
            var svr = clt.GetServer();
            var dblearn = svr.GetDatabase("learn");

            // var tlist = dblearn.GetCollection("receipts");

            var list = dblearn.GetCollection<Transaction>("receipts");

            var all = list.FindAll();
            var database = new ItemsetCollection();
            foreach (var transaction in all)
            {
                var itemset = new Itemset();
                foreach (var item in transaction.Items)
                {
                    itemset.Add(item);
                }

                database.Add(itemset);
            }

            var itemsunique = new Itemset();
            var i = 0;
            while (i < 50)
            {
                itemsunique.Add(i);
                i++;
            }

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
                    Console.Write("You either did not enter a number correctly or your number was too big! Try again...");
                    tries--;
                }
            }

            return ans;
        }

        private static void DataFromMysql()
        {
            Console.WriteLine("Can't do this right now... try again later");
        }
    }
}
