using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery777;
using System.Net;
using System.Diagnostics;

namespace LotteryEngine
{
    class MyLottery777Engine
    {
        #region Members
        List<Lottery777WinningResult> _LotteryHistoricResults = new List<Lottery777WinningResult>();
        public List<Lottery777WinningResult> WinningResults
        {
            get
            {
                return _LotteryHistoricResults;
            }
        }
        private List<List<NumberCommoness>> _RankedNumbers = new List<List<NumberCommoness>>();
        public List<List<NumberCommoness>> RankedNumbers
        {
            get
            {
                return _RankedNumbers;
            }
        }

        List<KeyValuePair<int, int>> _HotNumbersKVP = new List<KeyValuePair<int, int>>();
        List<Dictionary<int, int>> _HotNumbersPerNumber = null;
        List<List<KeyValuePair<int, int>>> _NumbersThatMutuallyAppearWithEachOther = null;

        PseudoPowerset _PowerSet = new PseudoPowerset();
        Random _Randomize = new Random();
        Stopwatch _Stopwatch = new Stopwatch();
        #endregion

        #region C'tor
        public MyLottery777Engine(string iLotteryResultsFilepath, bool iDownloadResultsFile)
        {
            string formerResultsFilename = string.Format("OldResults_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));

            if (iDownloadResultsFile)
            {
                //rename former file
                RenameFile("777.csv", formerResultsFilename);

                _Stopwatch.Start();
                DownloadFile(@"http://www.pais.co.il/777/Pages/last_Results.aspx?download=1", "777.csv");
                _Stopwatch.Stop();

                Console.WriteLine(string.Format("{0}: New results file download time: {1} seconds.", DateTime.Now, _Stopwatch.ElapsedMilliseconds / 1000));
            }

            

            ReadLottery777OfficialResultFile(iLotteryResultsFilepath);
            _HotNumbersKVP = PopulateHotNumbersList(GetOfficialCombinationsByDate(new DateTime(2002, 3, 13)), 70);

            _HotNumbersPerNumber = GetHotNumbersPerNumber();
            _NumbersThatMutuallyAppearWithEachOther = GetNumbersThatMutuallyAppearWithEachOther(_HotNumbersPerNumber);
        } 
        #endregion

        private void ReadLottery777OfficialResultFile(string iFilepath)
        {
            string path = string.Empty;

            if (Path.GetDirectoryName(iFilepath) == string.Empty)
            {
                path = Path.Combine(System.Environment.CurrentDirectory, iFilepath);
            }
            else
            {
                path = iFilepath;
            }

            if (Directory.Exists(Path.GetDirectoryName(path)))
            {
                using (StreamReader reader = new StreamReader(iFilepath, true))
                {
                    reader.ReadLine(); //skipping first line

                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] lineSplit = line.Split(',');
                        int[] numbers = new int[17];

                        string s = lineSplit[2].Replace("\"", "");

                        numbers[0] = int.Parse(lineSplit[2].Replace("\"", ""));
                        numbers[1] = int.Parse(lineSplit[3].Replace("\"", ""));
                        numbers[2] = int.Parse(lineSplit[4].Replace("\"", ""));
                        numbers[3] = int.Parse(lineSplit[5].Replace("\"", ""));
                        numbers[4] = int.Parse(lineSplit[6].Replace("\"", ""));
                        numbers[5] = int.Parse(lineSplit[7].Replace("\"", ""));
                        numbers[6] = int.Parse(lineSplit[8].Replace("\"", ""));
                        numbers[7] = int.Parse(lineSplit[9].Replace("\"", ""));
                        numbers[8] = int.Parse(lineSplit[10].Replace("\"", ""));
                        numbers[9] = int.Parse(lineSplit[11].Replace("\"", ""));
                        numbers[10] = int.Parse(lineSplit[12].Replace("\"", ""));
                        numbers[11] = int.Parse(lineSplit[13].Replace("\"", ""));
                        numbers[12] = int.Parse(lineSplit[14].Replace("\"", ""));
                        numbers[13] = int.Parse(lineSplit[15].Replace("\"", ""));
                        numbers[14] = int.Parse(lineSplit[16].Replace("\"", ""));
                        numbers[15] = int.Parse(lineSplit[17].Replace("\"", ""));
                        numbers[16] = int.Parse(lineSplit[18].Replace("\"", ""));

                        DateTime currDate;
                        bool isParsed = DateTime.TryParseExact(lineSplit[0].Replace("\"", ""), "dd/MM/yy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                        if (!isParsed)
                        {
                            isParsed = DateTime.TryParseExact(lineSplit[0].Replace("\"", ""), "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                            if (!isParsed)
                            {
                                isParsed = DateTime.TryParseExact(lineSplit[0].Replace("\"", ""), "dd/M/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                if (!isParsed)
                                {
                                    isParsed = DateTime.TryParseExact(lineSplit[0].Replace("\"", ""), "d/M/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                    if (!isParsed)
                                    {
                                        isParsed = DateTime.TryParseExact(lineSplit[0].Replace("\"", ""), "d/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                        if (!isParsed)
                                        {
                                            isParsed = DateTime.TryParseExact(lineSplit[0].Replace("\"", ""), "d/M/yy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                            if (!isParsed)
                                            {
                                                isParsed = DateTime.TryParseExact(lineSplit[0].Replace("\"", ""), "dd/M/yy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                                if (!isParsed)
                                                {
                                                    isParsed = DateTime.TryParseExact(lineSplit[0].Replace("\"", ""), "d/MM/yy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        

                        //0 - ID, 1 - Date, 2-7 numbers, 8 string number
                        _LotteryHistoricResults.Add(new Lottery777WinningResult() {
                            _LotteryRaffleID = lineSplit[1].Replace("\"", ""),
                                                     _LotteryDate = currDate,
                                                     _Numbers = numbers});
                        
                        i++;
                    }

                    //foreach (Lottery777WinningResult winningResult in _LotteryHistoricResults)
                    //{
                    //    List<int> numbersHit = new List<int>();
                    //    winningResult._HitCount = GetHitCountForTable(winningResult._Numbers, GetNumberOfOfficialCombinationsSinceDate(new DateTime(2009, 2, 28)), ref numbersHit);
                    //}
                }
            }
            else
            {
                throw new Exception(string.Format("Path {0} doesn't exist", Path.GetDirectoryName(iFilepath)));
            }
        }

        public List<int[]> GetOfficialCombinationsByDate(DateTime iUntil)
        {
            List<int[]> combinations = new List<int[]>();

            for (int i = 0; i < _LotteryHistoricResults.Count; i++)
            {
                if (_LotteryHistoricResults[i]._LotteryDate >= iUntil)
                {
                    combinations.Add(_LotteryHistoricResults[i]._Numbers);
                }
                else
                {
                    break;
                }
            }

            return combinations;
        }

        public int GetNumberOfOfficialCombinationsSinceDate(DateTime iUntil)
        {
            int counter = 0;

            for (int i = 0; i < _LotteryHistoricResults.Count; i++)
            {
                if (_LotteryHistoricResults[i]._LotteryDate >= iUntil)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }

            return counter;
        }

        public int GetHitCountForTable(int[] table, int iNumTablesToConsider, ref List<int> iNumbersHit)
        {
            int counter = 0;
            for (int i = 0; i < iNumTablesToConsider; i++)
            {
                int hitCount = CompareNumbersTables(table, _LotteryHistoricResults[i]._Numbers, ref iNumbersHit);
                if (hitCount > 2 || hitCount == 0)
                {
                    counter += 1;
                }
            }

            return counter;
        }

        private List<KeyValuePair<int, int>> PopulateHotNumbersList(List<int[]> iWinningResults, int iNumHotNumbers)
        {
            int[] winningResultsDispersion = new int[71];
            List<KeyValuePair<int, int>> numbers = new List<KeyValuePair<int, int>>();

            //creating initial dispersion
            for (int i = 0; i < iWinningResults.Count; i++)
            {
                for (int j = 0; j < iWinningResults[i].Length; j++)
                {
                    winningResultsDispersion[iWinningResults[i][j]]++;
                }
            }

            //choosing the hot numbers
            //create copy of original dispersion
            int[] tmp = (int[])winningResultsDispersion.Clone();
            for (int i = 0; i < iNumHotNumbers; i++)
            {
                KeyValuePair<int, int> currNumber = GetHighestValueFromArray(tmp);
                tmp[currNumber.Key] = 0;

                numbers.Add(currNumber);
            }

            return numbers;
        }

        private KeyValuePair<int, int> GetHighestValueFromArray(int[] iArray)
        {
            int location = 0;
            int value = 0;
            KeyValuePair<int, int> pair = new KeyValuePair<int, int>(-1, -1);
            for (int i = 1; i < iArray.Length; i++)
            {
                if (iArray[i] > pair.Value)
                {
                    pair = new KeyValuePair<int, int>(i, iArray[i]);
                }
            }

            return pair;
        }

        private int CompareNumbersTables(int[] iLotteryTable1, int[] iLotteryTable2, ref List<int> iNumbersHit)
        {
            int i = 0;
            int hitCount = 0;

            for (i = 0; i < iLotteryTable1.Length; i++)
            {
                if (iLotteryTable2.Contains(iLotteryTable1[i]))
                {
                    if (!iNumbersHit.Contains(iLotteryTable1[i]))
                    {
                        iNumbersHit.Add(iLotteryTable1[i]);
                    }
                    hitCount++;
                }
            }

            return hitCount;
        }

        public List<ChosenLottery777Table> GenerateLottery777Combinations3(List<int> iLeadingNumbers, bool iSaveToFile, string iFilename)
        {
            List<ChosenLottery777Table> generatedCombinations = new System.Collections.Generic.List<ChosenLottery777Table>();
            List<ChosenLottery777Table> chosenCombinations = new System.Collections.Generic.List<ChosenLottery777Table>();
            List<ChosenLottery777Table> createdCombinations = new System.Collections.Generic.List<ChosenLottery777Table>();
            List<int> leadingNumbers = new System.Collections.Generic.List<int>();
            for (int i = 0; i < 7; i++)
            {
                leadingNumbers.Add(_HotNumbersKVP[i].Key);
            }


            Dictionary<int[], int> dictCounter = new System.Collections.Generic.Dictionary<int[], int>();
            foreach (int number in leadingNumbers)
            {
                PseudoPowerset ps = new PseudoPowerset();
                List<int[]> combinations = new List<int[]>();
                ps.GenerateSubsets(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70 }, 3, ref combinations);
                combinations = combinations.Where(x => x.Contains(number)).ToList();

                foreach (int[] combination in combinations)
                {
                    foreach (Lottery777WinningResult winningResult in _LotteryHistoricResults)
                    {
                        if (winningResult._Numbers.Intersect(combination).Count() == combination.Count())
                        {
                            if (dictCounter.ContainsKey(combination))
                            {
                                dictCounter[combination]++;
                            }
                            else
                            {
                                dictCounter.Add(combination, 1);
                            }
                        }
                    }
                }

                dictCounter = dictCounter.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                //Generate combinations according to ranks
                //Take first 10 combinations (with most appearances)
                foreach (int[] combination in dictCounter.Keys)
                {
                    chosenCombinations.Add(new ChosenLottery777Table() { Numbers = combination });
                }

                foreach (ChosenLottery777Table combination in chosenCombinations)
                {
                    int[] currChosenCombination = CreateChosenCombination(combination.Numbers, number);

                    //Check hit count for currChosenCombination
                    int[] hitCount = GetCombinationHitCount(currChosenCombination);

                    if (hitCount[3] >= 1100 &&
                        hitCount[4] >= 310 &&
                        hitCount[5] >= 50 &&
                        hitCount[6] >= 5 &&
                        hitCount[7] == 0)
                    {
                        createdCombinations.Add(new ChosenLottery777Table()
                        {
                            Numbers = currChosenCombination,
                            HitCount = hitCount,
                            TotalHitcount = hitCount[3] +
                                hitCount[4] +
                                hitCount[5] +
                                hitCount[6]
                        });
                    }
                }

                generatedCombinations.AddRange(createdCombinations.Distinct().ToList());
            }

            generatedCombinations = generatedCombinations.Distinct(new DistinctChosenLottery777TableComparer()).OrderBy(x => x.TotalHitcount).Reverse().ToList();

            if (iSaveToFile)
            {
                foreach (ChosenLottery777Table combination in generatedCombinations)
                {
                    using (StreamWriter writer = new StreamWriter(iFilename, true))
                    {
                        string line = string.Format("{0},{1},{2},{3},{4},{5},{6},,{7},{8},{9},{10},{11},{12},{13},,{14}",
                                                    combination.Numbers[0],
                                                    combination.Numbers[1],
                                                    combination.Numbers[2],
                                                    combination.Numbers[3],
                                                    combination.Numbers[4],
                                                    combination.Numbers[5],
                                                    combination.Numbers[6],
                                                    combination.HitCount[0],
                                                    combination.HitCount[1],
                                                    combination.HitCount[2],
                                                    combination.HitCount[3],
                                                    combination.HitCount[4],
                                                    combination.HitCount[5],
                                                    combination.HitCount[6],
                                                    combination.TotalHitcount);

                        writer.WriteLine(line);
                    }
                }
            }

            return generatedCombinations;
        }

        private int[] CreateChosenCombination(int[] iBaseCombination, int iLeadingNumber)
        {
            List<int> chosen = new System.Collections.Generic.List<int>();
            int[] allNumbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70 };

            chosen.AddRange(iBaseCombination);


            //TBD: Should locate place of leading number...
            chosen.Add(_NumbersThatMutuallyAppearWithEachOther[iLeadingNumber - 1][0].Key);
            chosen.Add(_NumbersThatMutuallyAppearWithEachOther[iLeadingNumber - 1][1].Key);

            if (_NumbersThatMutuallyAppearWithEachOther[iBaseCombination[1] - 1].Count > 0)
            {
                chosen.Add(_NumbersThatMutuallyAppearWithEachOther[iBaseCombination[1] - 1][0].Key);
            }

            if (_NumbersThatMutuallyAppearWithEachOther[iBaseCombination[2] - 1].Count > 0)
            {
                chosen.Add(_NumbersThatMutuallyAppearWithEachOther[iBaseCombination[2] - 1][0].Key);
            }

            chosen = chosen.Distinct().ToList();

            if (chosen.Count < 7)
            {
                int[] bComplete = allNumbers.Except(chosen).ToArray();

                while (chosen.Count < 7)
                {
                    int index = _Randomize.Next(0, bComplete.Count() - 1);

                    chosen.Add(bComplete[index]);
                    bComplete = allNumbers.Except(chosen).ToArray();
                }
            }

            return chosen.OrderBy(x => x).Reverse().ToArray();
        }

        private List<Dictionary<int, int>> GetHotNumbersPerNumber()
        {
            Dictionary<List<int>, int> combinationsStats = new System.Collections.Generic.Dictionary<System.Collections.Generic.List<int>, int>();
            List<Dictionary<int, int>> common = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<int, int>>();

            for (int i = 1; i < 71; i++)
            {
                Dictionary<int, int> dict = new System.Collections.Generic.Dictionary<int,int>();

                foreach (Lottery777WinningResult result in _LotteryHistoricResults)
                {
                    if (result._Numbers.Contains(i))
                    {        
                        for (int j = 0; j < 17; j++)
                        {
                            if (dict.ContainsKey(result._Numbers[j]))
                            {
                                dict[result._Numbers[j]]++;
                            }
                            else
                            {
                                dict.Add(result._Numbers[j], 1);
                            }
                        }                        
                    }
                }

                common.Add(dict);
            }

            for (int i = 0; i < common.Count; i++)
            {
                common[i] = common[i].OrderBy(x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
            }

            return common;
        }

        private List<List<KeyValuePair<int, int>>> GetNumbersThatMutuallyAppearWithEachOther(List<Dictionary<int, int>> iHotNumbersPerNumbers)
        {
            List<List<KeyValuePair<int, int>>> commonCrossed = new System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, int>>>();

            //For each of the dictionaries (which represent numbers), cross reference with the 7 numbers that appear the most
            for (int i = 0; i < iHotNumbersPerNumbers.Count; i++)
            {
                List<KeyValuePair<int, int>> currNumber = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, int>>();
                for (int j = 0; j < 70; j++)
                {
                    if (j + 1 != i + 1)
                    {
                        List<KeyValuePair<int, int>> curr = iHotNumbersPerNumbers[j].ToList();
                        KeyValuePair<int, int> tuple = curr.Find(x => x.Key == i + 1);
                        
                        int index = curr.IndexOf(new KeyValuePair<int, int>(i + 1, tuple.Value));
                        if (index <= 5 && index != -1)
                        {
                            //add this tuple to a new dictionary containing only those numbers
                            currNumber.Add(iHotNumbersPerNumbers[i].OrderBy(x => x.Value).ToList().Find(x => x.Key == j + 1));
                        }
                    }
                }

                commonCrossed.Add(currNumber.OrderBy(x => x.Value).Reverse().ToList());
            }

            return commonCrossed;
        }

        public List<int[]> GenerateLottery777Combinations()
        {
            List<int[]> generatedCombinations = new System.Collections.Generic.List<int[]>();
            Dictionary<List<int>, int> combinationsStats = new System.Collections.Generic.Dictionary<System.Collections.Generic.List<int>, int>();

            for (int i = 0; i < _LotteryHistoricResults.Count; i++)
            {
                List<int[]> currCombinations = new System.Collections.Generic.List<int[]>();

                for (int j = 0; j < _LotteryHistoricResults.Count; j++)
                {
                    List<int> numbersHit = new System.Collections.Generic.List<int>();
                    if (CompareNumbersTables(_LotteryHistoricResults[i]._Numbers, _LotteryHistoricResults[j]._Numbers, ref numbersHit) != 17)
                    {
                        List<int> commonNumbers = GetCommonNumbers(_LotteryHistoricResults[i]._Numbers, _LotteryHistoricResults[j]._Numbers);
                        if (commonNumbers.Count >= 12)
                        {
                            if (!combinationsStats.ContainsKey(commonNumbers))
                            {
                                combinationsStats.Add(commonNumbers, 1);
                            }
                            else
                            {
                                combinationsStats[commonNumbers]++;
                            }
                        }
                    } 
                }
                //_PowerSet.GenerateSubsets(_LotteryHistoricResults[i]._Numbers, 7, ref currCombinations);



                //foreach (int[] combination in currCombinations)
                //{
                //    if (!combinationsStats.ContainsKey(combination))
                //    {
                //        combinationsStats.Add(combination, 1);
                //    }
                //    else
                //    {
                //        combinationsStats[combination]++;
                //    }
                //}
            }

            return generatedCombinations;
        }

        private List<int> GetCommonNumbers(int[] iCombinationA, int[] iCombinationB)
        {
            List<int> arr = new System.Collections.Generic.List<int>();

            foreach (int number in iCombinationA)
            {
                if (iCombinationB.Contains(number))
                {
                    arr.Add(number);
                }
            }


            return arr;
        }

        private int[] GetCombinationHitCount(int[] iCombination)
        {
            int[] hitCount = new int[8];

            foreach (Lottery777WinningResult winningResult in _LotteryHistoricResults)
            {
                hitCount[CalculateCombinationHitCount(iCombination, winningResult._Numbers)]++;
            }

            return hitCount;
        }

        private int CalculateCombinationHitCount(int[] iCombination, int[] iCheckedCombination)
        {
            int hitCount = 0;

            if (iCombination.Count() == 7)
            {
                foreach (int number in iCombination)
                {
                    if (iCheckedCombination.Contains(number))
                    {
                        hitCount++;
                    }
                }
            }
            else
            {
                throw new Exception("Combination length mismatch...");
            }

            return hitCount;
        }

        private void DownloadFile(string iUri, string iFilename)
        {
            WebClient Client = new WebClient();

            try
            {
                Client.DownloadFile(iUri, iFilename);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RenameFile(string iFilename, string iNewFilename)
        {
            if (File.Exists(iFilename))
            {
                try
                {
                    System.IO.File.Move(iFilename, iNewFilename);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                throw new Exception(string.Format("File: {0} doesn't exist...", iFilename));
            }
        }

        public int[] GetCountHotNumbersInWinningResults()
        {
            int[] counter = new int[8];

            foreach (Lottery777WinningResult result in _LotteryHistoricResults)
            {
                int currCounter = 0;
                for (int i = 0; i < 7; i++)
                {
                    if (result._Numbers.Contains(_HotNumbersKVP[i].Key))
                    {
                        currCounter++;
                    } 
                }

                counter[currCounter]++;
            }

            return counter;
        }
    }
}
