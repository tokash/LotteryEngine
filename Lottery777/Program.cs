using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotteryEngine;

namespace Lottery777
{
    public class Winnings
    {
        public int rafflenumber;
        public int winnings;
    }


    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            double startingCapital = 150000;

            sw.Start();
            MyLottery777Engine lotteryEngine = new MyLottery777Engine("777.csv", true);
            //MyLottery777Engine lotteryEngine = new MyLottery777Engine("777.csv", false);
            sw.Stop();

            Console.WriteLine(string.Format("Time to create Lottery777Engine: {0} seconds", sw.ElapsedMilliseconds / 1000));

            string generatedTablesfilename = string.Format("GeneratedTables_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            string winningTablesFilename = string.Format("WinningTables_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            string chosenTablesFilename = string.Format("ChosenTables_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            string chosenTablesFilename100 = string.Format("ChosenTables100_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            string allChosenTablesFilename = string.Format("AllChosenTables_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            //int[] counter = lotteryEngine.GetCountHotNumbersInWinningResults();
            //int counterTotal = 0;

            //foreach (int number in counter)
            //{
            //    counterTotal += number;
            //}
            List<ChosenLottery777Table> chosen = new List<ChosenLottery777Table>();

            sw.Reset();
            sw.Start();
            //lotteryEngine.GenerateLottery777Tables(70, chosenTablesFilename, ref chosen);
            //lotteryEngine.GenerateLottery777Tables(100, chosenTablesFilename100, ref chosen);
            //List<int[]> chosenTables = lotteryEngine.ReadChosenTablesFromFile("WinningCombinationsOrderedByHighestNumberOf5Hits.csv");
            //List<int[]> chosenTables = lotteryEngine.ReadChosenTablesFromFile("WinningCombinationsOrderedByDiscovery.csv");
            //List<int[]> chosenTables = lotteryEngine.ReadChosenTablesFromFile("WinningCombinationsOrderedByHighestNumberOf5Hits.csv");
            //List<int[]> chosenTables = lotteryEngine.ReadChosenTablesFromFile("WinningCombinationsOrderedByHighestNumberOf6Hits.csv");
            //List<int[]> chosenTables = lotteryEngine.ReadChosenTablesFromFile("WinningCombinationsOrderedByHighestNumberOf7Hits.csv");
            //List<int[]> chosenTables = lotteryEngine.ReadChosenTablesFromFile("WinningCombinationsOrderedByHighestNumberOfHits.csv");
            List<int[]> chosenTables = lotteryEngine.ReadChosenTablesFromFile("67hitsSmall.csv");
            chosenTables = chosenTables.Distinct(new DistinctItemComparer()).ToList();
            //List<int[]> chosenTables = lotteryEngine.ReadChosenTablesFromFile("444.csv");

            int[] hitCount = new int[8];
            int winnings = 0;
            List<Winnings> lstWinnings = new List<Winnings>();
            for (int i = 0; i < 100; i++)
            {
                //winnings = lotteryEngine.CalculateWinnings(lotteryEngine.WinningResults[i]._Numbers, i + 1, chosenTables.Take(60).ToList(), ref hitCount);
                winnings = lotteryEngine.CalculateWinnings(lotteryEngine.WinningResults[i]._Numbers, i + 1, chosenTables.ToList(), ref hitCount);
                lstWinnings.Add(new Winnings() { rafflenumber = i + 1, winnings = winnings });
                //startingCapital = startingCapital - 7 * 60 + winnings;
                startingCapital = startingCapital - 7 * chosenTables.Count + winnings;
                hitCount = Enumerable.Repeat(0, 8).ToArray();

                //if (i % 10 == 0)
                //{
                    //Console.WriteLine("Remaining capital after {0} raffles: {1}", i + 1, startingCapital);
                //}
            }

            //float winnings = lotteryEngine.CalculateWinnings(lotteryEngine.WinningResults[0]._Numbers, chosenTables, ref hitCount);
            //hitCount = Enumerable.Repeat(0, 8).ToArray();

            //winnings = lotteryEngine.CalculateWinnings(lotteryEngine.WinningResults[1]._Numbers, chosenTables, ref hitCount);
            //hitCount = Enumerable.Repeat(0, 8).ToArray();

            //winnings = lotteryEngine.CalculateWinnings(lotteryEngine.WinningResults[2]._Numbers, chosenTables, ref hitCount);
            //float totalSpent = 7 * 60 * 100;
            float totalSpent = 7 * chosenTables.Count * 100;
            float totalWon = 0;
            foreach (Winnings item in lstWinnings)
            {
                totalWon += item.winnings;
            }

            float ratio = totalWon / totalSpent;
            double netWon = totalWon * 0.7;               

            Console.WriteLine(string.Format("Spent: {0}, Won: {1}, Ratio: {2}", totalSpent, totalWon, ratio));
            Console.WriteLine(string.Format("Spent: {0}, Net Won: {1}, Ratio: {2}", totalSpent, netWon, netWon/totalSpent));
            
            sw.Stop();

            //Console.WriteLine(string.Format("Time taken to generate lottery 777 numbers: {0} hours", sw.Elapsed));
        }
    }
}
