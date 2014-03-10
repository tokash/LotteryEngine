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

            //number, percentage
            wantedDispersion.Add(1, 100);
            //wantedDispersion.Add(2, 17);
            //wantedDispersion.Add(3, 16);
            //wantedDispersion.Add(4, 10);
            //wantedDispersion.Add(5, 5);
            //wantedDispersion.Add(6, 5);
            //wantedDispersion.Add(7, 5);
            //wantedDispersion.Add(8, 15);

            lotteryEngine.GetAllLotteryTablesCombinations3(generatedTablesfilename, true, 2700, wantedDispersion, true, winningTablesFilename, chosenTablesFilename);
            //List<ChosenLotteryTable> records = lotteryEngine.ReadChosenCombinations("ChosenTables_10.03.2014.11.04.15.3573.csv");
            //int[] results = lotteryEngine.CheckHitCountForChosenCombinations(new int[]{17, 21, 23, 24, 26, 35}, 6 , records);

        }
    }
}
