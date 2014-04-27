using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotteryEngine;

namespace Lottery777
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            MyLottery777Engine lotteryEngine = new MyLottery777Engine("777.csv", false);
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
            lotteryEngine.GenerateLottery777Tables(6756, chosenTablesFilename100, ref chosen);
            sw.Stop();

            Console.WriteLine(string.Format("Time taken to generate lottery 777 numbers: {0} hours", sw.Elapsed));
        }
    }
}
