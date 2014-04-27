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
        List<KeyValuePair<int, int>> _HotNumbersKVPLast5 = new List<KeyValuePair<int, int>>();
        List<KeyValuePair<int, int>> _HotNumbersKVPLast10 = new List<KeyValuePair<int, int>>();
        List<KeyValuePair<int, int>> _HotNumbersKVPLast20 = new List<KeyValuePair<int, int>>();
        List<KeyValuePair<int, int>> _HotNumbersKVPLast50 = new List<KeyValuePair<int, int>>();
        List<KeyValuePair<int, int>> _HotNumbersKVPLast70 = new List<KeyValuePair<int, int>>();
        List<KeyValuePair<int, int>> _HotNumbersKVPLast100 = new List<KeyValuePair<int, int>>();
        //List<KeyValuePair<int, int>> _HotNumbersKVPLast250 = new List<KeyValuePair<int, int>>();
        //List<KeyValuePair<int, int>> _HotNumbersKVPLast500 = new List<KeyValuePair<int, int>>();
        //List<KeyValuePair<int, int>> _HotNumbersKVPLast1000 = new List<KeyValuePair<int, int>>();
        //List<Dictionary<int, int>> _HotNumbersPerNumber = null;
        List<List<KeyValuePair<int, int>>> _NumbersThatMutuallyAppearWithEachOther = null;

        int[] set = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70 };

        //PseudoPowerset _PowerSet = new PseudoPowerset();
        Random _Randomize = new Random();
        Stopwatch _Stopwatch = new Stopwatch();
        #endregion

        #region C'tor
        public MyLottery777Engine(string iLotteryResultsFilepath, bool iDownloadResultsFile)
        {
            string formerResultsFilename = string.Format("OldResults_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));

            if (iDownloadResultsFile)
            {
                if (File.Exists("777.csv"))
                {
                    //rename former file
                    RenameFile("777.csv", formerResultsFilename); 
                }

                _Stopwatch.Start();
                DownloadFile(@"http://www.pais.co.il/777/Pages/last_Results.aspx?download=1", "777.csv");
                _Stopwatch.Stop();

                Console.WriteLine(string.Format("{0}: New results file download time: {1} seconds.", DateTime.Now, _Stopwatch.ElapsedMilliseconds / 1000));
            }

            

            ReadLottery777OfficialResultFile(iLotteryResultsFilepath);
            //_HotNumbersKVP = PopulateHotNumbersList(GetOfficialCombinationsByDate(new DateTime(2002, 3, 13)), 70);
            //_HotNumbersKVPLast5 = PopulateHotNumbersList(GetLastNOfficialCombinations(5), 70);
            //_HotNumbersKVPLast10 = PopulateHotNumbersList(GetLastNOfficialCombinations(10), 70);
            //_HotNumbersKVPLast20 = PopulateHotNumbersList(GetLastNOfficialCombinations(20), 70);
            //_HotNumbersKVPLast50 = PopulateHotNumbersList(GetLastNOfficialCombinations(50), 70);
            //_HotNumbersKVPLast70 = PopulateHotNumbersList(GetLastNOfficialCombinations(70), 70);
            //_HotNumbersKVPLast100 = PopulateHotNumbersList(GetLastNOfficialCombinations(100), 70);
            //_HotNumbersKVPLast250 = PopulateHotNumbersList(GetLastNOfficialCombinations(250), 70);
            //_HotNumbersKVPLast500 = PopulateHotNumbersList(GetLastNOfficialCombinations(500), 70);
            //_HotNumbersKVPLast1000 = PopulateHotNumbersList(GetLastNOfficialCombinations(1000), 70);

            //SelectNumbersFromAllGroups();

            //_HotNumbersPerNumber = GetHotNumbersPerNumber();
            //_NumbersThatMutuallyAppearWithEachOther = GetNumbersThatMutuallyAppearWithEachOther2(_HotNumbersPerNumber);
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

        public List<int[]> GetLastNOfficialCombinations(int iNumCombinations)
        {
            List<int[]> combinations = new List<int[]>();

            if (iNumCombinations <= _LotteryHistoricResults.Count)
            {
                for (int i = 0; i < iNumCombinations; i++)
                {
                    combinations.Add(_LotteryHistoricResults[i]._Numbers);
                }
            }
            else
            {
                for (int i = 0; i < _LotteryHistoricResults.Count; i++)
                {
                    combinations.Add(_LotteryHistoricResults[i]._Numbers);
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

                if (currNumber.Value != 0)
                {
                    numbers.Add(currNumber); 
                }
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
                    int[] hitCount = GetWinningCombinationHitCount(currChosenCombination);

                    if (hitCount[3] >= 1100 &&
                        hitCount[4] >= 310 &&
                        hitCount[5] >= 50 &&
                        hitCount[6] >= 5 &&
                        hitCount[7] == 0)
                    {
                        createdCombinations.Add(new ChosenLottery777Table()
                        {
                            Numbers = currChosenCombination,
                            HitCountArray = hitCount,
                            HitCount = hitCount[3] +
                                hitCount[4] +
                                hitCount[5] +
                                hitCount[6]
                        });
                    }
                }

                generatedCombinations.AddRange(createdCombinations.Distinct().ToList());
            }

            generatedCombinations = generatedCombinations.Distinct(new DistinctChosenLottery777TableComparer()).OrderBy(x => x.HitCount).Reverse().ToList();

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
                                                    combination.HitCountArray[0],
                                                    combination.HitCountArray[1],
                                                    combination.HitCountArray[2],
                                                    combination.HitCountArray[3],
                                                    combination.HitCountArray[4],
                                                    combination.HitCountArray[5],
                                                    combination.HitCountArray[6],
                                                    combination.HitCount);

                        writer.WriteLine(line);
                    }
                }
            }

            return generatedCombinations;
        }

        public List<ChosenLottery777Table> GenerateLottery777Combinations2(bool iSaveToFile, string iFilename)
        {
            List<ChosenLottery777Table> generatedCombinations = new System.Collections.Generic.List<ChosenLottery777Table>();
            List<List<int>> chosenLists = new System.Collections.Generic.List<System.Collections.Generic.List<int>>();
            List<ChosenLottery777Table> chosenCombinations = new System.Collections.Generic.List<ChosenLottery777Table>();
            List<ChosenLottery777Table> createdCombinations = new System.Collections.Generic.List<ChosenLottery777Table>();
            List<int> leadingNumbers = new System.Collections.Generic.List<int>();
            for (int i = 0; i < 7; i++)
            {
                leadingNumbers.Add(_HotNumbersKVP[i].Key);
            }

            foreach (int number in leadingNumbers)
            {
                List<KeyValuePair<int, int>> l = _NumbersThatMutuallyAppearWithEachOther[number -1];
                List<int> l1 = new System.Collections.Generic.List<int>();
                foreach (KeyValuePair<int, int> item in l)
                {
                    l1.Add(item.Key);
                }

                List<int> numbers = new System.Collections.Generic.List<int>();
                numbers.Add(number);
                numbers.AddRange(l1.ToArray());
                chosenLists.Add(numbers);
            }

            PseudoPowerset ps = new PseudoPowerset();

            foreach (List<int> chosenList in chosenLists)
	        {
		        List<int[]> combinations = new List<int[]>();
                ps.GenerateSubsets(chosenList.ToArray(), 7, ref combinations);

                foreach (int[] combination in combinations)
                {
                    chosenCombinations.Add(new ChosenLottery777Table() { Numbers = combination });
                }
	        }

            for (int i = 0; i < chosenCombinations.Count; i++)
            {
                //Check hit count for currChosenCombination
                int[] hitCount = GetWinningCombinationHitCount(chosenCombinations[i].Numbers);
                int totalHitcount = hitCount[3] +
                            hitCount[4] +
                            hitCount[5] +
                            hitCount[6];


                if (totalHitcount >= 1620)
                {
                    createdCombinations.Add(new ChosenLottery777Table()
                    {
                        Numbers = chosenCombinations[i].Numbers,
                        HitCountArray = hitCount,
                        HitCount = totalHitcount
                    });
                }

                //if (hitCount[3] >= 1190 &&
                //    hitCount[4] >= 330 &&
                //    hitCount[5] >= 50 &&
                //    hitCount[6] >= 5 &&
                //    hitCount[7] == 0)
                //{
                //    createdCombinations.Add(new ChosenLottery777Table()
                //    {
                //        Numbers = chosenCombinations[i].Numbers,
                //        HitCount = hitCount,
                //        TotalHitcount = hitCount[3] +
                //            hitCount[4] +
                //            hitCount[5] +
                //            hitCount[6]
                //    });
                //}

                Console.WriteLine(i);
            }

            generatedCombinations.AddRange(createdCombinations.Distinct().ToList());

            generatedCombinations = generatedCombinations.Distinct(new DistinctChosenLottery777TableComparer()).OrderBy(x => x.HitCount).Reverse().ToList();

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
                                                    combination.HitCountArray[0],
                                                    combination.HitCountArray[1],
                                                    combination.HitCountArray[2],
                                                    combination.HitCountArray[3],
                                                    combination.HitCountArray[4],
                                                    combination.HitCountArray[5],
                                                    combination.HitCountArray[6],
                                                    combination.HitCount);

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
                        if (index <= 7 && index != -1)
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

        private List<List<KeyValuePair<int, int>>> GetNumbersThatMutuallyAppearWithEachOther2(List<Dictionary<int, int>> iHotNumbersPerNumbers)
        {
            List<List<KeyValuePair<int, int>>> commonCrossed = new System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, int>>>();

            //For each of the dictionaries (which represent numbers), cross reference with the 7 numbers that appear the most
            for (int i = 0; i < iHotNumbersPerNumbers.Count; i++)
            {
                List<KeyValuePair<int, int>> currNumber = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, int>>();
                
                for (int j = 0; j < 70; j++)
                {
                    List<KeyValuePair<int, int>> curr = iHotNumbersPerNumbers[j].ToList();

                    if (j + 1 != i + 1)
                    {
                        KeyValuePair<int, int> tuple = curr.Find(x => x.Key == i + 1);

                        int index = curr.IndexOf(new KeyValuePair<int, int>(i + 1, tuple.Value));

                        List<int> list = new System.Collections.Generic.List<int>();
                        foreach (KeyValuePair<int,int> item in currNumber)
                        {
                            list.Add(item.Key);
                        }

                        if (index <= 7 && index != -1)
                        {
                            if (currNumber.Count > 0)
                            {
                                if (!currNumber.Contains(tuple) && DoesMutuallyAppearWith(curr[0].Key, list, iHotNumbersPerNumbers))
                                {
                                    //add this tuple to a new dictionary containing only those numbers
                                    currNumber.Add(iHotNumbersPerNumbers[i].ToList().Find(x => x.Key == curr[0].Key));
                                }
                            }
                            else
                            {
                                currNumber.Add(iHotNumbersPerNumbers[i].ToList().Find(x => x.Key == curr[0].Key)); 
                            }
                        }
                    }
                }
                currNumber.Add(iHotNumbersPerNumbers[i].Single(x => x.Key == i + 1));
                if (currNumber.Count >=3 )
                {
                    if (!commonCrossed.Contains(currNumber))
                    {
                        commonCrossed.Add(currNumber.OrderBy(x => x.Value).Reverse().ToList()); 
                    }
                }
            }

            return commonCrossed;
        }

        private bool DoesMutuallyAppearWith(int iNumber, List<int> iNumbers, List<Dictionary<int, int>> iHotNumbersPerNumbers)
        {
            bool doesAppear = false;
            int counter = 0;
            int index = -1;
            int opositeIndex = -1;

            //Need to check if iNumber appears with all the numbers in iNumbers inside iHotNumbersPerNumbers
            foreach (int number in iNumbers)
            {
                List<KeyValuePair<int, int>> curr = iHotNumbersPerNumbers[number - 1].ToList();
                
                for (int i = 0; i < curr.Count; i++)
                {
                    if (curr[i].Key == iNumber )
                    {
                        index = i;
                        break;
                    }
                }


                //Checking index for the oposite case - when curr appears in iNumbers list
                if (index <= 7 && index != -1 && index != 0)
                {
                    List<KeyValuePair<int, int>> opositeCurr = iHotNumbersPerNumbers[iNumber - 1].ToList();

                    for (int i = 0; i < opositeCurr.Count; i++)
                    {
                        if (opositeCurr[i].Key == curr[0].Key)
                        {
                            opositeIndex = i;
                            break;
                        }
                    }
                }

                if (index <= 7 && index != -1 && opositeIndex <=7 && opositeIndex != -1)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }

            if (counter == iNumbers.Count)
            {
                doesAppear = true;
            }

            return doesAppear;
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

        private int[] GetWinningCombinationHitCount(int[] iCombination)
        {
            int[] hitCount = new int[8];

            foreach (Lottery777WinningResult winningResult in _LotteryHistoricResults)
            {
                hitCount[CalculateCombinationHitCount(iCombination, winningResult._Numbers)]++;
            }

            return hitCount;
        }

        private int[] GetCombinationsHitCount(List<int[]> iCombinations, int iNumLastWinningCombinations)
        {
            int[] hitCount = new int[8];
            List<Lottery777WinningResult> wantedWinningResults = _LotteryHistoricResults.Take(iNumLastWinningCombinations).ToList();

            foreach (int[] combination in iCombinations)
            {
                foreach (Lottery777WinningResult winningCombination in wantedWinningResults)
                {
                    hitCount[CalculateCombinationHitCount(combination, winningCombination._Numbers)]++; 
                }
            }

            return hitCount;
        }

        public int[] GetMethodical9CombinationsHitCount(List<int[]> iCombinations, int iNumLastWinningCombinations)
        {
            int[] hitCount = new int[10];
            List<Lottery777WinningResult> wantedWinningResults = _LotteryHistoricResults.Take(iNumLastWinningCombinations).ToList();
            List<int[]> chosen = new System.Collections.Generic.List<int[]>();
            int counter = 0;

            foreach (int[] combination in iCombinations)
            {
                for (int i = 0; i < iNumLastWinningCombinations; i++)
                {
                    foreach (int number in combination)
                    {
                        if (_LotteryHistoricResults[i]._Numbers.Contains(number))
                        {
                            counter++;
                        }
                    }

                    switch (counter)
                    {
                        case 0:
                            hitCount[0] += 36;
                            break;
                        case 1:
                            hitCount[1] += 8;
                            break;
                        case 2:
                            hitCount[2] += 1;
                            break;
                        case 3:
                            hitCount[3] += 15;
                            break;
                        case 4:
                            hitCount[4] += 10;
                            hitCount[3] += 20;
                            break;
                        case 5:
                            hitCount[5] += 6;
                            hitCount[4] += 20;
                            hitCount[3] += 10;
                            break;
                        case 6:
                            hitCount[6] += 3;
                            hitCount[5] += 18;
                            hitCount[4] += 15;
                            break;
                        case 7:
                            hitCount[7] += 1;
                            hitCount[6] += 14;
                            hitCount[5] += 21;
                            break;
                        case 8:
                            hitCount[8] += 8;
                            hitCount[7] += 28;
                            break;
                        case 9:
                            hitCount[9] += 36;
                            break;
                    }

                    counter = 0;
                }    
            }

            return hitCount;
        }

        public int[] GetMethodical8CombinationsHitCount(List<int[]> iCombinations, int iNumLastWinningCombinations)
        {
            int[] hitCount = new int[9];
            List<Lottery777WinningResult> wantedWinningResults = _LotteryHistoricResults.Take(iNumLastWinningCombinations).ToList();
            List<int[]> chosen = new System.Collections.Generic.List<int[]>();
            int counter = 0;

            foreach (int[] combination in iCombinations)
            {
                for (int i = 0; i < iNumLastWinningCombinations; i++)
                {
                    foreach (int number in combination)
                    {
                        if (_LotteryHistoricResults[i]._Numbers.Contains(number))
                        {
                            counter++;
                        }
                    }

                    switch (counter)
                    {
                        case 0:
                            hitCount[0] += 8;
                            break;
                        case 1:
                            hitCount[1] += 1;
                            break;
                        case 2:
                            hitCount[2] = 0;
                            break;
                        case 3:
                            hitCount[3] += 5;
                            break;
                        case 4:
                            hitCount[4] += 4;
                            hitCount[3] += 4;
                            break;
                        case 5:
                            hitCount[5] += 3;
                            hitCount[4] += 5;                         
                            break;
                        case 6:
                            hitCount[6] += 2;
                            hitCount[5] += 6;                            
                            break;
                        case 7:
                            hitCount[7] += 1;
                            hitCount[6] += 7;                            
                            break;
                        case 8:
                            hitCount[8] += 8;
                            break;                 
                    }

                    counter = 0;
                }
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

        private int CalculateMethodicalCombinationHitCount(int[] iCombination, int[] iCheckedCombination)
        {
            int hitCount = 0;

            if (iCombination.Count() == 9)
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

        private List<int> SelectNumbers(List<KeyValuePair<int, int>> iNumbers, int iResultsToConsider)
        {
            List<int> numbersList = new System.Collections.Generic.List<int>();

            foreach (KeyValuePair<int, int> number in iNumbers)
            {
                if (IsEligibleForSelection(number, iResultsToConsider))
                {
                    numbersList.Add(number.Key);
                }
            }

            return numbersList;
        }

        private List<int> SelectNumbersExt(List<KeyValuePair<int, int>> iNumbers, int iResultsToConsider)
        {
            List<int> numbersList = new System.Collections.Generic.List<int>();

            foreach (KeyValuePair<int, int> number in iNumbers)
            {
                numbersList.Add(number.Key);
            }

            return numbersList;
        }

        private List<int> SelectNumbersFromAllGroups()
        {
            int count = 0;
            //List<int> Last5 = SelectNumbers(_HotNumbersKVPLast5, 5);
            List<int> Last10 = SelectNumbers(_HotNumbersKVPLast10, 10);
            List<int> Last20 = SelectNumbers(_HotNumbersKVPLast20, 20);
            List<int> Last50 = SelectNumbers(_HotNumbersKVPLast50, 50);
            List<int> Last70 = SelectNumbers(_HotNumbersKVPLast70, 70);
            List<int> Last100 = SelectNumbers(_HotNumbersKVPLast100, 100);
            //List<int> Last250 = SelectNumbers(_HotNumbersKVPLast250, 250);
            //List<int> Last500 = SelectNumbers(_HotNumbersKVPLast500, 500);
            //List<int> Last1000 = SelectNumbers(_HotNumbersKVPLast1000, 1000);
            List<int> chosenNumbers = new System.Collections.Generic.List<int>();
            int currNumber = -1;
            string chosenTablesfilename = string.Format("Chosen_{0}.csv", DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss.ffff"));

            //List<int> union = Last20.Union(Last50).Union(Last70).Union(Last100).Union(Last10).OrderBy(x => x).Reverse().ToList();

            //foreach (int number in union)
            //{
            //    currNumber = number;
                //if (Last5.Contains(number))
                //{
                //    count++;
                //}

                //if (Last10.Contains(number))
                //{
                //    count++;
                //}

                //if (Last20.Contains(number))
                //{
                //    count++;
                //}

                //if (Last50.Contains(number))
                //{
                //    count++;
                //}

                //if (Last70.Contains(number))
                //{
                //    count++;
                //}

                //if (Last100.Contains(number))
                //{
                //    count++;
                //}

                //if (Last250.Contains(number))
                //{
                //    count++;
                //}

                //if (Last500.Contains(number))
                //{
                //    count++;
                //}

                //if (Last1000.Contains(number))
                //{
                //    count++;
                //}

            //    if (count >= 4)
            //    {
            //        chosenNumbers.Add(currNumber);
            //    }
            //    count = 0;
            //}

            //Compare to actual winning results
            //List<int> intersectChosenLast20 = chosenNumbers.Intersect(Last20).ToList();
            //List<int[]> last20 = GenerateAllSubsetsOfSizeN(intersectChosenLast20.ToArray(), 7);
            //int[] last20HitCount = GetCombinationsHitCount(last20, 20);

            //List<int> intersectLast50_70 = Last50.Intersect(Last70).ToList();
            //List<int[]> last50_70 = GenerateAllSubsetsOfSizeN(intersectLast50_70.ToArray(), 7);
            //int[] last50_70HitCount = GetCombinationsHitCount(last50_70, 70);
            //int[] last50_70Winnings = CalcWinnings(last50_70HitCount);

            //List<int> interse     ctLast50_70_100 = Last50.Intersect(Last70).Intersect(Last100).ToList();
            //List<int[]> last50_70_100 = GenerateAllSubsetsOfSizeN(intersectLast50_70_100.ToArray(), 7);
            //int[] last50_70_100HitCount = GetCombinationsHitCount(last50_70_100, 75);
            //int[] last50_70_100Winnings = CalcWinnings(last50_70_100HitCount);

            //List<int> intersectLast50_100 = Last50.Intersect(Last100).ToList();
            //List<int[]> last50_100 = GenerateAllSubsetsOfSizeN(intersectLast50_100.ToArray(), 7);
            //int[] last50_100HitCount = GetCombinationsHitCount(last50_100, 100);
            //int[] last50_100Winnings = CalcWinnings(last50_100HitCount);

            //List<int> intersectLast70_100 = Last70.Intersect(Last100).ToList();
            //List<int[]> last70_100 = GenerateAllSubsetsOfSizeN(intersectLast70_100.ToArray(), 7);
            //int[] last70_100HitCount = GetCombinationsHitCount(last70_100, 100);
            //int[] last70_100Winnings = CalcWinnings(last70_100HitCount);

            //List<int> intersectLast100_250 = Last100.Intersect(Last250).ToList();
            //List<int[]> last100_250 = GenerateAllSubsetsOfSizeN(intersectLast100_250.ToArray(), 7);
            //int[] last100_250HitCount = GetCombinationsHitCount(last100_250, 250);
            //int[] last100_250Winnings = CalcWinnings(last100_250HitCount);

            //List<int> intersectChosenLast50 = chosenNumbers.Intersect(Last50).ToList();
            //List<int[]> last50 = GenerateAllSubsetsOfSizeN(intersectChosenLast50.ToArray(), 7);
            //int[] last50HitCount = GetCombinationsHitCount(last50, 1);
            //int[] last50Winnings = CalcWinnings(last50HitCount);

            //List<ChosenLottery777MethodicalTable> chosenMethodical50 = GenerateMethodical9Tables(Last50.ToArray());

            //List<int> intersectChosenLast70 = chosenNumbers.Intersect(Last70).ToList();
            //List<int[]> last70 = GenerateAllSubsetsOfSizeN(intersectChosenLast70.ToArray(), 7);
            //int[] last70HitCount = GetCombinationsHitCount(last70, 1);
            //int[] last70Winnings = CalcWinnings(last70HitCount);

            //List<ChosenLottery777MethodicalTable> chosenMethodical70 = GenerateMethodical9Tables(Last70.ToArray());
            //WriteChosenMethodicalCombinationsToFile(chosenTablesfilename, chosenMethodical70);
            //chosenMethodical50.AddRange(chosenMethodical70);

            //List<int> intersectChosenLast100 = chosenNumbers.Intersect(Last100).ToList();
            //List<int[]> last100 = GenerateAllSubsetsOfSizeN(intersectChosenLast100.ToArray(), 7);
            //int[] last100HitCount = GetCombinationsHitCount(last100, 1);
            //int[] last100Winnings = CalcWinnings(last100HitCount);

            //List<ChosenLottery777MethodicalTable> chosenMethodical100 = GenerateMethodical9Tables(Last100.ToArray());
            //chosenMethodical50.AddRange(chosenMethodical100);
            //chosenMethodical50 = chosenMethodical50.Distinct(new DistinctChosenLottery777MethodicalTableComparer()).ToList();

            //WriteChosenMethodicalCombinationsToFile(chosenTablesfilename, chosenMethodical50);

            //List<int> intersect50_70_100 = Last50.Intersect(Last70).Intersect(Last100).ToList();

            #region NotUsed
            //List<int> unionChosen50_100 = intersectChosenLast100.Union(intersectChosenLast50).ToList();

            //List<int> intersectChosenLast250 = chosenNumbers.Intersect(Last250).ToList();
            //List<int[]> last250 = GenerateAllSubsetsOfSizeN(intersectChosenLast250.ToArray(), 7);
            //int[] last250HitCount = GetCombinationsHitCount(last250, 1);
            //int[] last250Winnings = CalcWinnings(last250HitCount);

            //List<int> intersectChosenLast500 = chosenNumbers.Intersect(Last500).ToList();
            //List<int[]> last500 = GenerateAllSubsetsOfSizeN(intersectChosenLast500.ToArray(), 7);
            //int[] last500HitCount = GetCombinationsHitCount(last500, 1);
            //int[] last500Winnings = CalcWinnings(last500HitCount);

            //List<int> intersectChosenLast1000 = chosenNumbers.Intersect(Last1000).ToList();

             
            #endregion


            //List<ChosenLottery777MethodicalTable> chosenMethodical = GenerateMethodical9Tables(SelectNumbersExt(_HotNumbersKVPLast10.Take(56).ToList(), 10).ToArray(), chosenTablesfilename);
            //List<ChosenLottery777MethodicalTable> chosenMethodical = GenerateMethodical8Tables(SelectNumbersExt(_HotNumbersKVPLast10.Take(60).ToList(), 10).ToArray(), chosenTablesfilename);
            //List<int[]> combinationsChosen = GenerateAllSubsetsOfSizeN(chosenNumbers.ToArray(), 7);
            //int[] chosenHitCount = GetCombinationsHitCount(combinationsChosen, 1);
            //int[] chosenWinnings = CalcWinnings(chosenHitCount);

            //WriteChosenMethodical8CombinationsToFile(chosenTablesfilename, chosenMethodical);

            return chosenNumbers;
        }

        private void WriteChosenMethodical9CombinationsToFile(string iFilename, List<ChosenLottery777MethodicalTable> iChosenMethodicalTables)
        {
            foreach (ChosenLottery777MethodicalTable combination in iChosenMethodicalTables)
            {
                using (StreamWriter writer = new StreamWriter(iFilename, true))
                {
                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},,{9},{10},{11},{12},{13},{14},{15}",
                                                combination.Numbers[0],
                                                combination.Numbers[1],
                                                combination.Numbers[2],
                                                combination.Numbers[3],
                                                combination.Numbers[4],
                                                combination.Numbers[5],
                                                combination.Numbers[6],
                                                combination.Numbers[7],
                                                combination.Numbers[8],
                                                combination.HitCountArray[3],
                                                combination.HitCountArray[4],
                                                combination.HitCountArray[5],
                                                combination.HitCountArray[6],
                                                combination.HitCountArray[7],
                                                combination.HitCountArray[8],
                                                combination.HitCountArray[9]
                                                );

                    writer.WriteLine(line);
                }
            }
        }

        private void WriteChosenMethodical8CombinationsToFile(string iFilename, List<ChosenLottery777MethodicalTable> iChosenMethodicalTables)
        {
            foreach (ChosenLottery777MethodicalTable combination in iChosenMethodicalTables)
            {
                using (StreamWriter writer = new StreamWriter(iFilename, true))
                {
                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},,{8},{9},{10},{11},{12},{13}",
                                                combination.Numbers[0],
                                                combination.Numbers[1],
                                                combination.Numbers[2],
                                                combination.Numbers[3],
                                                combination.Numbers[4],
                                                combination.Numbers[5],
                                                combination.Numbers[6],
                                                combination.Numbers[7],
                                                combination.HitCountArray[3],
                                                combination.HitCountArray[4],
                                                combination.HitCountArray[5],
                                                combination.HitCountArray[6],
                                                combination.HitCountArray[7],
                                                combination.HitCountArray[8]
                                                );

                    writer.WriteLine(line);
                }
            }
        }

        private List<ChosenLottery777MethodicalTable> GenerateMethodical8Tables(int[] iNumbers, string iFilename)
        {
            //List<int[]> methodical9 = GenerateAllSubsetsOfSizeN(iNumbers, 9, iFilename);
            //List<ChosenLottery777MethodicalTable> chosen = new System.Collections.Generic.List<ChosenLottery777MethodicalTable>();
            //int i = 0;
            //foreach (int[] combination in methodical9)
            //{
            //    List<int[]> tmp = new System.Collections.Generic.List<int[]>();
            //    tmp.Add(combination);

            //List<int[]> generated9 = GenerateAllSubsetsOfSizeN(tmp[0].ToArray(), 7);
            //foreach (int[] combination7 in generated9)
            //{
            //    List<int[]> tmp7 = new System.Collections.Generic.List<int[]>();
            //    tmp7.Add(combination7);
            //    int[] currMethodicalHitCount = GetCombinationsHitCount(tmp7, 1);

            //if (currMethodicalHitCount[7] > 0 || currMethodicalHitCount[6] > 0)
            //{
            //int[] hitCount = GetMethodicalCombinationsHitCount(tmp, 5);
            //if (/*hitCount[5] >= 1 || hitCount[6] >= 1 ||*/ hitCount[7] >= 1 /*|| hitCount[8] >= 1 || hitCount[9] >= 1*/)
            //{
            //    ChosenLottery777MethodicalTable curr = new ChosenLottery777MethodicalTable() { Numbers = combination, HitCount = hitCount };

            //    if (!chosen.Contains(curr, new DistinctChosenLottery777MethodicalTableComparer()))
            //    {
            //        chosen.Add(curr);
            //    } 
            //}
            //}
            //}
            //    Console.WriteLine("{0}", i);
            //    i++;
            //}

            return GenerateAllSubsetsOfSizeN(iNumbers, 8, iFilename);
        }

        private List<ChosenLottery777MethodicalTable> GenerateMethodical9Tables(int[] iNumbers, string iFilename)
        {
            //List<int[]> methodical9 = GenerateAllSubsetsOfSizeN(iNumbers, 9, iFilename);
            //List<ChosenLottery777MethodicalTable> chosen = new System.Collections.Generic.List<ChosenLottery777MethodicalTable>();
            //int i = 0;
            //foreach (int[] combination in methodical9)
            //{
            //    List<int[]> tmp = new System.Collections.Generic.List<int[]>();
            //    tmp.Add(combination);

                //List<int[]> generated9 = GenerateAllSubsetsOfSizeN(tmp[0].ToArray(), 7);
                //foreach (int[] combination7 in generated9)
                //{
                //    List<int[]> tmp7 = new System.Collections.Generic.List<int[]>();
                //    tmp7.Add(combination7);
                //    int[] currMethodicalHitCount = GetCombinationsHitCount(tmp7, 1);

                    //if (currMethodicalHitCount[7] > 0 || currMethodicalHitCount[6] > 0)
                    //{
                        //int[] hitCount = GetMethodicalCombinationsHitCount(tmp, 5);
                        //if (/*hitCount[5] >= 1 || hitCount[6] >= 1 ||*/ hitCount[7] >= 1 /*|| hitCount[8] >= 1 || hitCount[9] >= 1*/)
                        //{
                        //    ChosenLottery777MethodicalTable curr = new ChosenLottery777MethodicalTable() { Numbers = combination, HitCount = hitCount };

                        //    if (!chosen.Contains(curr, new DistinctChosenLottery777MethodicalTableComparer()))
                        //    {
                        //        chosen.Add(curr);
                        //    } 
                        //}
                    //}
                //}
            //    Console.WriteLine("{0}", i);
            //    i++;
            //}

            return GenerateAllSubsetsOfSizeN(iNumbers, 9, iFilename);
        }

        private int[] CalcWinnings(int[] iHitCount)
        {
            int[] winnings = new int[9];

            winnings[0] = iHitCount[0] * 5;
            winnings[3] = iHitCount[3] * 5;
            winnings[4] = iHitCount[4] * 20;
            winnings[5] = iHitCount[5] * 50;
            winnings[6] = iHitCount[6] * 500;
            winnings[7] = iHitCount[7] * 70000;

            winnings[8] = winnings[0] + winnings[3] + winnings[4] + winnings[5] + winnings[6] + winnings[7];

            return winnings;
        }

        //Either x/y >= 33% or x*3/y >= 0.8
        private bool IsEligibleForSelection(KeyValuePair<int, int> iNumber, int iResultsToConsider)
        {
            bool isEligible = false;

            if ((double)iNumber.Value/iResultsToConsider >= 0.33 || (iResultsToConsider >10 && (double)(iNumber.Value * 3)/iResultsToConsider >= 0.8))
            {
                isEligible = true;
            }

            return isEligible;
        }

        private List<ChosenLottery777MethodicalTable> GenerateAllSubsetsOfSizeN(int[] iNumbers, int iSubsetSize, string iFilename)
        {
            List<int[]> combinations = new List<int[]>();
            List<ChosenLottery777MethodicalTable> chosen = new System.Collections.Generic.List<ChosenLottery777MethodicalTable>();
            GenerateSubsets(iNumbers, iSubsetSize, iFilename, ref chosen);

            return chosen;
        }

        public void GenerateSubsets(int[] set, int k, string iFilename, ref List<ChosenLottery777MethodicalTable> iChosen)
        {
            int[] subset = new int[k];
            ProcessLargerSubsets(set, subset, 0, 0, iFilename, ref iChosen);

            return;
        }

        void ProcessLargerSubsets(int[] set, int[] subset, int subsetSize, int nextIndex, string iFilename, ref List<ChosenLottery777MethodicalTable> iChosen)
        {
            if (subsetSize == subset.Length)
            {
                List<int[]> tmp = new System.Collections.Generic.List<int[]>();
                tmp.Add((int[])subset.Clone());

                int[] hitCount = GetMethodical8CombinationsHitCount(tmp, 10);
                //int[] hitCount = GetMethodical9CombinationsHitCount(tmp, 5);
                if (/*hitCount[5] >= 1 || hitCount[6] >= 1 ||*/ hitCount[7] >= 1 /*|| hitCount[8] >= 1 || hitCount[9] >= 1*/)
                {
                    ChosenLottery777MethodicalTable curr = new ChosenLottery777MethodicalTable() { Numbers = (int[])subset.Clone(), HitCountArray = hitCount };

                    if (/*(!iChosen.Contains(curr, new DistinctChosenLottery777MethodicalTableComparer()) &&
                        hitCount[4] >= 19) ||*/

                        (!iChosen.Contains(curr, new DistinctChosenLottery777MethodicalTableComparer()) &&
                        hitCount[5] > 9) ||

                        (!iChosen.Contains(curr, new DistinctChosenLottery777MethodicalTableComparer()) &&
                        hitCount[6] >= 11))
                    {
                        iChosen.Add(curr);

                        //if (iChosen.Count == 30)
                        //{
                        //    return;
                        //}
                    }

                    //if ((!iChosen.Contains(curr, new DistinctChosenLottery777MethodicalTableComparer()) &&
                    //    hitCount[4] >= 55) ||

                    //    (!iChosen.Contains(curr, new DistinctChosenLottery777MethodicalTableComparer()) &&
                    //    hitCount[5] >= 45) ||

                    //    (!iChosen.Contains(curr, new DistinctChosenLottery777MethodicalTableComparer()) &&
                    //    hitCount[6] >= 20))
                    //{
                    //    iChosen.Add(curr);

                    //    if (iChosen.Count == 30)
                    //    {
                    //        return;
                    //    }
                    //}
                }

                ////write to file
                //using (StreamWriter writer = new StreamWriter(iFilename, true))
                //{
                //    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                //                                subset[0],
                //                                subset[1],
                //                                subset[2],
                //                                subset[3],
                //                                subset[4],
                //                                subset[5],
                //                                subset[6],
                //                                subset[7],
                //                                subset[8]
                //                                );

                //    writer.WriteLine(line);
                //}
            }
            else
            {

                for (int j = nextIndex; j < set.Length; j++)
                {
                    subset[subsetSize] = set[j];
                    ProcessLargerSubsets(set, subset, subsetSize + 1, j + 1, iFilename, ref iChosen);

                    //if (iChosen.Count >= 30)
                    //{
                    //    return;
                    //}
                }
            }
        }

        public void GenerateSubsets2(int[] set, int k, string iFilename, ref List<ChosenLottery777Table> iChosen)
        {
            int[] subset = new int[k];
            ProcessLargerSubsets2(set, subset, 0, 0, iFilename, ref iChosen);

            return;
        }

        void ProcessLargerSubsets2(int[] set, int[] subset, int subsetSize, int nextIndex, string iFilename, ref List<ChosenLottery777Table> iChosen)
        {
            if (subsetSize == subset.Length)
            {
                List<int[]> tmp = new System.Collections.Generic.List<int[]>();
                tmp.Add((int[])subset.Clone());

                ChosenLottery777Table curr = new ChosenLottery777Table() { Numbers = (int[])subset.Clone(), HitCount = 0 };
                int[] hitCountArray = new int[8];
                int lastWonRaffle = -1;
                int hitCount = GetHitCountForLastNResults(curr.Numbers, 5, 30, ref hitCountArray, ref lastWonRaffle);
                curr.HitCount = hitCount;
                curr.HitCountArray = hitCountArray;
                if (hitCount >= 5 && lastWonRaffle >=8 && !iChosen.Contains(curr, new DistinctChosenLottery777TableComparer()))
                {
                    iChosen.Add(curr);
                }
                
            }
            else
            {

                for (int j = nextIndex; j < set.Length; j++)
                {
                    subset[subsetSize] = set[j];
                    ProcessLargerSubsets2(set, subset, subsetSize + 1, j + 1, iFilename, ref iChosen);
                }
            }
        }

        private int GetHitCountForLastNResults(int[] iGeneratedTable, int iTolerance, int iNumberOfResultsToConsider, ref int[] oHitCountArray, ref int oLastWonRaffle)
        {
            int hitCount = 0;

            for (int i = 0; i < iNumberOfResultsToConsider; i++)
            {
                int currHitCount = iGeneratedTable.Intersect(_LotteryHistoricResults[i]._Numbers).Count();
                if (currHitCount >= iTolerance)
                {
                    oHitCountArray[currHitCount]++;
                    hitCount++;

                    if (oLastWonRaffle == -1)
                    {
                        oLastWonRaffle = i + 1; //The last time these numbers appeared in a winning result (actually the number in the sequence of winning results since they are read from file)
                    }
                }
            }

            return hitCount;
        }

        private int GetHitCountForLastNResults(int[] iGeneratedTable, int iTolerance, int iNumberOfResultsToConsider, ref int[] oHitCountArray, ref List<int> oWinningRaffleTracking)
        {
            int hitCount = 0;

            for (int i = 0; i < iNumberOfResultsToConsider; i++)
            {
                int currHitCount = iGeneratedTable.Intersect(_LotteryHistoricResults[i]._Numbers).Count();
                if (currHitCount >= iTolerance)
                {
                    oHitCountArray[currHitCount]++;
                    hitCount++;

                    oWinningRaffleTracking.Add(i +1);
                }
            }

            return hitCount;
        }

        public void GenerateLottery777Tables(int iNumResultsToConsider, string iFilename, ref List<ChosenLottery777Table> oChosen)
        {
            Stopwatch sw = new Stopwatch();
            
            List<ChosenLottery777Table> generatedPossibilites = new System.Collections.Generic.List<ChosenLottery777Table>();

            for (int i = 0; i < iNumResultsToConsider; i++)
            {
                //sw.Start();
                GenerateSubsets3(_LotteryHistoricResults[i]._Numbers, 7, iFilename, ref generatedPossibilites);
                //sw.Stop();
                
                //Console.Write(string.Format("Generating all combinations of 7 from 17 numbers took: {0}", sw.Elapsed));
                //Console.Write("\r");
                //sw.Reset();

                //sw.Start();
                foreach (ChosenLottery777Table item in generatedPossibilites)
                {
                    
                    int[] hitCountArray = new int[8];
                    List<int> winningRaffleTracking = new System.Collections.Generic.List<int>();
                    int hitCount = GetHitCountForLastNResults(item.Numbers, 5, iNumResultsToConsider, ref hitCountArray, ref winningRaffleTracking);

                    if (hitCount >= 85)// && !iChosen.Contains(item, new DistinctChosenLottery777TableComparer()))
                    {
                        item.HitCount = hitCount;
                        item.HitCountArray = hitCountArray;
                        item.WinningRafflesTracking = winningRaffleTracking;
                        oChosen.Add(item);
                    }
                }

                //sw.Stop();
                //Console.WriteLine(string.Format("Going over all combinations took: {0}", sw.Elapsed));
                //Console.Write("\r");
                //sw.Reset();

                generatedPossibilites.Clear();
            }

            WriteChosenCombinationsToFile(iFilename, oChosen);

        }

        private Dictionary<int[], int> GetTablesCount()
        {
            Dictionary<int[], int> tableCount = new System.Collections.Generic.Dictionary<int[], int>();

            foreach (Lottery777WinningResult table in _LotteryHistoricResults)
            {
                if (tableCount.ContainsKey(table._Numbers))
                {
                    tableCount[table._Numbers]++;
                }
                else
                {
                    tableCount.Add(table._Numbers, 1);
                }
            }

            return tableCount;
        }

        public void GenerateSubsets3(int[] set, int k, string iFilename, ref List<ChosenLottery777Table> iChosen)
        {
            int[] subset = new int[k];
            ProcessLargerSubsets3(set, subset, 0, 0, iFilename, ref iChosen);

            return;
        }

        void ProcessLargerSubsets3(int[] set, int[] subset, int subsetSize, int nextIndex, string iFilename, ref List<ChosenLottery777Table> iChosen)
        {
            if (subsetSize == subset.Length)
            {
                ChosenLottery777Table curr = new ChosenLottery777Table() { Numbers = (int[])subset.Clone(), HitCount = 0 };
                iChosen.Add(curr);
            }
            else
            {

                for (int j = nextIndex; j < set.Length; j++)
                {
                    subset[subsetSize] = set[j];
                    ProcessLargerSubsets3(set, subset, subsetSize + 1, j + 1, iFilename, ref iChosen);
                }
            }
        }

        private void WriteChosenCombinationsToFile(string iFilename, List<ChosenLottery777Table> iChosenTables)
        {
            foreach (ChosenLottery777Table combination in iChosenTables)
            {
                using (StreamWriter writer = new StreamWriter(iFilename, true))
                {
                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},,{7},{8},{9},{10},{11}",
                                                combination.Numbers[0],
                                                combination.Numbers[1],
                                                combination.Numbers[2],
                                                combination.Numbers[3],
                                                combination.Numbers[4],
                                                combination.Numbers[5],
                                                combination.Numbers[6],
                                                combination.HitCountArray[3],
                                                combination.HitCountArray[4],
                                                combination.HitCountArray[5],
                                                combination.HitCountArray[6],
                                                combination.HitCountArray[7]
                                                );
                    string raffleTracking = string.Empty;
                    foreach (int raffleID in combination.WinningRafflesTracking)
                    {
                        raffleTracking += string.Format("{0},", raffleID);
                    }

                    line += ",," + raffleTracking;
                    writer.WriteLine(line);
                }
            }
        }
    }
}
