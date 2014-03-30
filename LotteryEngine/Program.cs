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
            MyLotteryEngine lotteryEngine = new MyLotteryEngine("Lotto.csv");
            Dictionary<int, int> wantedDispersion = new Dictionary<int, int>();
            string generatedTablesfilename = string.Format("GeneratedTables_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            string winningTablesFilename = string.Format("WinningTables_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            string chosenTablesFilename = string.Format("ChosenTables_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            string allChosenTablesFilename = string.Format("AllChosenTables_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));
            double wins = 0;
            double losses = 0;
                
            //number, percentage
            wantedDispersion.Add(1, 34);
            wantedDispersion.Add(2, 33);
            wantedDispersion.Add(3, 33);
            wantedDispersion.Add(4, 11);
            wantedDispersion.Add(5, 10);
            wantedDispersion.Add(6, 7);
            wantedDispersion.Add(7, 2);
            wantedDispersion.Add(8, 2);
            //wantedDispersion.Add(9, 2);

            //lotteryEngine.GetAllLotteryTablesCombinations3(generatedTablesfilename, true, 168, wantedDispersion, true, winningTablesFilename, true, allChosenTablesFilename, chosenTablesFilename);
            lotteryEngine.GetAllLotteryTablesCombinations3(generatedTablesfilename, false, 5, wantedDispersion, false, winningTablesFilename, false, allChosenTablesFilename, chosenTablesFilename);
            //List<ChosenLotteryTable> records = lotteryEngine.ReadChosenCombinations("ChosenTables_15.03.2014.16.29.35.3881.csv");
            //double profit = lotteryEngine.CalcWinningsForTables(records, lotteryEngine.WinningResults.Take(550).ToList(), new int[]{0, 0, 0, 10, 33, 46, 117, 497, 3846, 500000, 0}, 2.9, ref wins, ref losses);
            //int[] results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 1, 3, 6, 9, 34, 37 }, 3, records);
            //int[] results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 17, 21, 23, 24, 26, 35 }, 2, records);
            //results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 3, 6, 17, 23, 27, 32 }, 6, records);
            //results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 4, 8, 9, 22, 29, 33 }, 6, records);
            //results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 5, 6, 11, 21, 22, 28 }, 2, records);
            //results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 3, 8, 23, 26, 28, 29 }, 2, records);
            //results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 3,7,14,21,27,37 }, 6, records);
            //results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 1,6,10,16,21,22 }, 4, records);
            //results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 2, 5, 18, 20, 23, 29}, 3, records);
            //results = lotteryEngine.CheckHitCountForChosenCombinations(new int[] { 16, 22, 26, 29, 36, 37 }, 3, records);

       }
    }
}
