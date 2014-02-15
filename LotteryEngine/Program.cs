using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            MyLotteryEngine lotteryEngine = new MyLotteryEngine("Lotto.csv", 14);
            int i = 0;
            int n = 38;
            int k = 6;
            //lotteryEngine.GenerateMultipleTables(30);
            //lotteryEngine.WriteTablesToFile(string.Format("{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.fff")));
            //int[] numberStatistics = lotteryEngine.FindAllRepetitions();

            lotteryEngine.ReadCombinationsFile("Combinations.csv", true);
            List<ShortLotteryTable> allCombinations = lotteryEngine.Combinations;

            Console.WriteLine("Writing to file...");
            for (i = 0; i < allCombinations.Count; ++i)
            {
                using (StreamWriter writer = new StreamWriter("dilutedCombinations.txt", true))
                {
                    string line = string.Format("{0},{1},{2},{3},{4},{5}", allCombinations[i]._Numbers[0],
                                                                           allCombinations[i]._Numbers[1],
                                                                           allCombinations[i]._Numbers[2],
                                                                           allCombinations[i]._Numbers[3],
                                                                           allCombinations[i]._Numbers[4],
                                                                           allCombinations[i]._Numbers[5]);
                    writer.WriteLine(line);
                }
            }

            //Combination c = new Combination(n, k);

            
            //long numCombinations = Combination.Choose(n-1, k);
            //Console.WriteLine(string.Format("\nWith n={0} and k={1} there are {2} combination elements.",n-1 ,k ,numCombinations));

            //Console.WriteLine("\nThe elements are:");
            //for (i = 0; i < numCombinations; ++i)
            //{
            //    Console.WriteLine(i + ": " + c.ToString());

            //    using (StreamWriter writer = new StreamWriter("Combinations.txt", true))
            //    {
            //        writer.WriteLine(c.ToString());
            //    }
            //    c = c.Successor();
                
            //}
            
            string winningsFilename = string.Format("Winnings.{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.fff"));
            string numbersFilename = string.Format("Numbers.{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.fff"));
            //while (i < 100)
            //{
            //    lotteryEngine.GetLotteryTables(14, winningsFilename, numbersFilename);
            //    i++;
            //}
            //lotteryEngine.WriteTablesToFile(string.Format("{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.fff")));


            }
    }
}
