namespace DataTransformer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using MongoDB.Driver;
    using MongoDB.Bson;

    /// <summary>
    /// Main Console Program Class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry Point To Program
        /// </summary>
        /// <param name="args">command line arguments (not used)</param>
        [STAThread]
        static void Main(string[] args)
        {

            Console.WriteLine("===================================");
            Console.WriteLine("  Welcome to DataTransformer v1.0");
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
        /// Shows main Menu of options and returns true if user intends to continue
        /// </summary>
        /// <returns>Boolean indicates whether user wants to continue</returns>
        private static bool ShowMainMenu()
        {
            Console.WriteLine("\n***************************************************************");
            Console.WriteLine("\n\tMAIN MENU OPTIONS: ");
            Console.WriteLine();
            Console.WriteLine("\ta)\tProcess data into MongoDB (JSON or BSON format)    ");
            Console.WriteLine("\tb)\tProcess data into mysql        ");
            Console.WriteLine("\tc)\tCheck out a little test        ");
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
                    DataToMongo();
                    cont = PromptToContinue();
                    break;
                case "b":
                case "B":
                    DataToMysql();
                    cont = PromptToContinue();
                    break;
                case "c":
                case "C":
                    LittleTest();
                    cont = PromptToContinue();
                    break;
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

        /// <summary>
        /// Just a test method
        /// </summary>
        private static void LittleTest()
        {
            Console.WriteLine("This is under construction... sorry");
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
        /// Grabs some parameters from user and inserts data into mongoDB collection
        /// </summary>
        private static void DataToMongo()
        {
            var datafile = GetDatafilePath();

            //var filetxtArray = File.ReadAllLines(datafile);

            //foreach (var s in filetxtArray)
            //{
            //    var index = s.Substring(0, s.IndexOf(","));
            //}

            // Connect to Mongo server and inserts into collection named "receipts"
            const string ConnStr = "mongodb://localhost:27017";
            var clt = new MongoClient(ConnStr);
            var svr = clt.GetServer();
            var dblearn = svr.GetDatabase("learn");

            // var receipts = new MongoCollection<Transaction>(,);
            var transCollection = dblearn.GetCollection<Transaction>("receipts");

            using (var reader = new StreamReader(datafile))
            {
                string line;
                string[] row;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        row = line.Split(',');
                        MongoInsertAddRow(row, transCollection);
                    }
                }
            }


        }

        /// <summary>
        /// Just shows a File Browser dialogue in order to user to select file
        /// </summary>
        /// <returns>Returns string of the full data file path</returns>
        private static string GetDatafilePath()
        {
            var datafile = string.Empty;
            var openfilewindow = new PickAFile();
            openfilewindow.Closing += (sender, args) => { datafile = openfilewindow.SelectedFilePath; };
            openfilewindow.ShowDialog();

            Console.WriteLine("\nThe data file selected was: \n" + datafile);
            Console.WriteLine("Now transforming data and inserting into a database...");
            return datafile;
        }

        // End dataToMongo


        /// <summary>
        /// Process the row string and creates a new Transaction which is feed into mongoDB
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="collection">The collection in mongo we're inserting to</param>
        private static void MongoInsertAddRow(string[] row, MongoCollection<Transaction> collection )
        {
            var str = ArrayToString(row);
            Console.WriteLine("Parsed and inserted row#{0} which had items: {1}", row[0], str.Substring(str.IndexOf(",") + 1));

            collection.Insert(new Transaction(row));
        }

        /// <summary>
        /// Just a helper method that strings an array
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A string version of the array</returns>
        private static string ArrayToString(string[] row)
        {
            var result = string.Join(",", row);
            return result;
        }

        /// <summary>
        /// Grabs some parameters from user and inserts data into mysql
        /// </summary>
        private static void DataToMysql()
        {
            var datafile = GetDatafilePath();
        }
    }
}
