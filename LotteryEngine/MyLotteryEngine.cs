using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryEngine
{
    class MyLotteryEngine
    {
        #region Members
        List<LotteryTable> _LotteryTables = new List<LotteryTable>();
        List<List<LotteryTable>> _Batches = new List<List<LotteryTable>>();
        List<NumberStatistics> _FactorHotNumbers = new List<NumberStatistics>();
        List<LotteryWinningResult> _LotteryHistoricResults = new List<LotteryWinningResult>();
        public List<LotteryWinningResult> WinningResults
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

        Random _RandomNumbersGenerator = new Random();
        List<ShortLotteryTable> _Combinations = new List<ShortLotteryTable>();
        public List<ShortLotteryTable> Combinations
        {
            get
            {
                return _Combinations;
            }
        }

        List<Dispersion> _Dispersion = null;

        //Generate 5 batches of 6 lottery numbers
        //Replace all numbers which are not "strong", with strong number according to their factor

        //Compare results with "real" results from csv file

        //Most popular numbers since the latest change, 6 numbers and one strong number (14/5/2011) until

        //int[] Numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 ,26 , 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37 };
        //1   2   3   4   5   6   7   8   9   10  11  12  13  14  15  16  17  18  19  20  21  22  23  24  25  26  27  28  29  30  31  32  33  34  35  36  37
        int[] NumbersDispersion = new int[] { 0, 53, 56, 57, 44, 41, 35, 52, 53, 41, 46, 48, 55, 40, 37, 48, 53, 48, 41, 51, 47, 60, 38, 47, 51, 54, 63, 48, 46, 48, 53, 44, 43, 52, 49, 54, 57, 53 };
        List<int> _HotNumbers = new List<int>();
        List<KeyValuePair<int, int>> _HotNumbersKVP = new List<KeyValuePair<int, int>>();
        List<int> _RegularNumbers = new List<int>();
        List<int> _ColdNumbers = new List<int>();
        int[][] _NumbersCommoness = null;
        
        int _NumberOfRuffles = 308;

        int[] _StringHotNumbers = new int[] { 1, 3, 5 };
        int[] _Repetitions = null;

        int _WantedTables = 0; 
        #endregion

        #region C'tor
        public MyLotteryEngine(string iLotteryResultsFilepath)
        {
            for (int i = 1; i < NumbersDispersion.Length; i++)
            {
                //int factor = (int)Math.Floor((NumbersDispersion[i] / (double)_NumberOfRuffles / _WantedTables) * 1000);
                double factorPercentage = NumbersDispersion[i] / (double)_NumberOfRuffles;
                int factor = 0;
                if (factorPercentage > 0.176)
                {
                    if (_WantedTables < 50)
                    {
                        factor = (int)Math.Round((factorPercentage / (_WantedTables - 6)) * 1000 * (0.3 + factorPercentage));
                    }
                    else if (_WantedTables < 100)
                    {
                        factor = (int)Math.Round((factorPercentage / (_WantedTables - 6)) * 1000 * (0.8 + factorPercentage));
                    }
                    else if (_WantedTables < 200)
                    {
                        factor = (int)Math.Round((factorPercentage / (_WantedTables - 6)) * 1000 * (1.1 + factorPercentage));
                    }
                    _FactorHotNumbers.Add(new NumberStatistics() { Number = i, Factor = factor });
                }
                else
                {
                    factor = (int)Math.Round((NumbersDispersion[i] / (double)_NumberOfRuffles / (_WantedTables - 6)) * 1000);
                }
            }

            ReadLotteryOfficialResultFile(iLotteryResultsFilepath);
            _HotNumbersKVP = PopulateHotNumbersList(GetOfficialCombinationsByDate(new DateTime(2009, 2, 28)), 6);
        } 
        #endregion

        public void ReadCommonessFile(string iNumberCommonessFile)
        {
            _NumbersCommoness = GetAllNumbersCommoness(iNumberCommonessFile);

            for (int i = 1; i < _NumbersCommoness.Length; i++)
            {
                _RankedNumbers.Add(SetRank(i));
            }
        }

        public void Find2CombinationsInside6Combinations(List<int[]> iCombinations, int iMaxNumber, List<List<NumberCommoness>> iNumberCommoness, string iFilename, bool iAppendToFile)
        {
            int i = 0, j = 0, counter = 0;
            bool deletionFailedFlag = false;

            //need to delete file if existing
            try
            {
                DeleteFile(iFilename);
            }
            catch (Exception)
            {
                deletionFailedFlag = true;
            }

            if (iAppendToFile || deletionFailedFlag == false)
            {
                for (i = 1; i < iMaxNumber; i++)
                {
                    for (j = i + 1; j < iMaxNumber; j++)
                    {
                        foreach (int[] combination in iCombinations)
                        {
                            if (combination.Contains(i) && combination.Contains(j))
                            {
                                counter++;
                            }
                        }

                        //write result to file if counter > 0
                        if (counter > 0)
                        {
                            using (StreamWriter writer = new StreamWriter(iFilename, true))
                            {
                                int rank = iNumberCommoness[i - 1].Find(x => x.Number == j).Rank;
                                string line = string.Format("{0},{1},{2},{3}", i, j, counter, rank);
                                writer.WriteLine(line);
                            }
                        }

                        //reset counter
                        counter = 0;
                    }
                }
            }
        }

        private static void DeleteFile(string iFilename)
        {
            if (File.Exists(iFilename))
            {
                try
                {
                    File.Delete(iFilename);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to delete file: {0}.", iFilename));
                    Console.WriteLine(string.Format("Delete file exception: {0}.", ex.ToString()));
                    throw ex;
                }
            }
        }

        public void Find2CombinationsInside6Combinations(List<int[]> iCombinations, int iMaxNumber, string iFilename, bool iAppendToFile)
        {
            int i = 0, j = 0, counter = 0;
            bool deletionFailedFlag = false;

            //need to delete file if existing
            try
            {
                DeleteFile(iFilename);
            }
            catch (Exception)
            {
                deletionFailedFlag = true;
            }

            if (iAppendToFile || deletionFailedFlag == false)
            {
                for (i = 1; i < iMaxNumber; i++)
                {
                    for (j = i + 1; j < iMaxNumber; j++)
                    {

                        foreach (int[] combination in iCombinations)
                        {
                            if (combination.Contains(i) && combination.Contains(j))
                            {
                                counter++;
                            }
                        }

                        //write result to file if counter > 0
                        if (counter > 0)
                        {
                            using (StreamWriter writer = new StreamWriter(iFilename, true))
                            {
                                string line = string.Format("{0},{1},{2}", i, j, counter);
                                writer.WriteLine(line);
                            }
                        }

                        //reset counter
                        counter = 0;
                    }
                }
            }
        }

        public void Find3CombinationsInside6Combinations(List<int[]> iCombinations, int iMaxNumber, string iFilename)
        {
            int i = 0, j = 0, k = 0, counter = 0;

            for (i = 1; i < iMaxNumber; i++)
            {
                for (j= i + 1; j< iMaxNumber; j++)
                {
                    for (k = j+1; k < iMaxNumber; k++)
                    {
                        foreach (int[] combination in iCombinations)
                        {
                            if (combination.Contains(i) && combination.Contains(j) && combination.Contains(k))
                            {
                                counter++;
                            }
                        }

                        //write result to file if counter > 0
                        if (counter > 0)
                        {
                            using (StreamWriter writer = new StreamWriter(iFilename, true))
                            {
                                string line = string.Format("{0},{1},{2},{3}", i, j, k, counter);
                                writer.WriteLine(line);
                            }
                        }

                        //reset counter
                        counter = 0;
                    }
                }
            }
        }

        public void Find4CombinationsInside6Combinations(List<int[]> iCombinations, int iMaxNumber, string iFilename)
        {
            int i = 0, j = 0, k = 0, t = 0, counter = 0;

            for (i = 1; i < iMaxNumber; i++)
            {
                for (j = i + 1; j < iMaxNumber; j++)
                {
                    for (k = j + 1; k < iMaxNumber; k++)
                    {
                        for (t = k+1; t<iMaxNumber ; t++)
                        {
                            foreach (int[] combination in iCombinations)
                            {
                                if (combination.Contains(i) && combination.Contains(j) && combination.Contains(k) && combination.Contains(t))
                                {
                                    counter++;
                                }
                            }

                            //write result to file if counter > 0
                            if (counter > 0)
                            {
                                using (StreamWriter writer = new StreamWriter(iFilename, true))
                                {
                                    string line = string.Format("{0},{1},{2},{3},{4}", i, j, k, t, counter);
                                    writer.WriteLine(line);
                                }
                            }

                            //reset counter
                            counter = 0;    
                        }
                        
                    }
                }
            }
        }

        public void Find5CombinationsInside6Combinations(List<int[]> iCombinations, int iMaxNumber, string iFilename)
        {
            int i = 0, j = 0, k = 0, t = 0, v = 0, counter = 0;

            for (i = 1; i < iMaxNumber; i++)
            {
                for (j = i + 1; j < iMaxNumber; j++)
                {
                    for (k = j + 1; k < iMaxNumber; k++)
                    {
                        for (t = k + 1; t < iMaxNumber; t++)
                        {
                            for (v = t + 1;  v < iMaxNumber; v++)
                            {
                                foreach (int[] combination in iCombinations)
                                {
                                    if (combination.Contains(i) && combination.Contains(j) && combination.Contains(k) && combination.Contains(t) && combination.Contains(v))
                                    {
                                        counter++;
                                    }
                                }

                                //write result to file if counter > 0
                                if (counter > 0)
                                {
                                    using (StreamWriter writer = new StreamWriter(iFilename, true))
                                    {
                                        string line = string.Format("{0},{1},{2},{3},{4},{5}", i, j, k, t, v, counter);
                                        writer.WriteLine(line);
                                    }
                                }

                                //reset counter
                                counter = 0;    
                            }
                            
                        }

                    }
                }
            }
        }

        public List<ShortLotteryTable> GetMatchingCombinations(List<ShortLotteryTable> iCombinations, List<int[]> iNumberBlends)
        {
            List<ShortLotteryTable> matches = null;
            bool isWinningTable = false;

            if (iCombinations!= null && iCombinations.Count > 0)
            {
                if (iNumberBlends != null && iNumberBlends.Count > 0)
                {
                    matches = new List<ShortLotteryTable>();
                    foreach (ShortLotteryTable table in iCombinations)
                    {
                        for (int i = 0; i < iNumberBlends.Count; i++)
                        {
                            int cold = 0;
                            int regular = 0;
                            int hot = 0;

                            for (int j = 0; j < table._Numbers.Length; j++)
                            {
                                int currNumber = table._Numbers[j];
                                if (_HotNumbers.Contains(currNumber))
                                {
                                    hot++;
                                }
                                else if(_RegularNumbers.Contains(currNumber))
                                {
                                    regular++;
                                }
                                else
                                {
                                    cold++;
                                }
                            }

                            if (cold == iNumberBlends[i][0] && regular == iNumberBlends[i][1] && hot == iNumberBlends[i][2])
                            {
                                //foreach (LotteryWinningResult winningTable in _LotteryHistoricResults)
                                //{
                                //    if (Compare6Numbers(winningTable._Numbers, table._Numbers))
                                //    {
                                //        isWinningTable = true;
                                //        break;
                                //    }
                                //}

                                //if (!isWinningTable)
                                //{
                                    matches.Add(table);
                                //}
                            }
                            isWinningTable = false;
                        }
                    }
                }
            }

            return matches;
        }

        public bool CompareIntArray(int[] arr1, int[] arr2)
        {
            int counter = 0;
            bool isIdentical = false;
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] == arr2[i])
                {
                    counter++;
                }
            }

            if (counter == arr1.Length)
            {
                isIdentical = true;
            }

            return isIdentical;
        }

        public void GetNumbersDispersionInWinningResults(List<LotteryWinningResult> iLotteryResults, int iResultsToConsider, string iFilename)
        {
            int coldNumbersCounter = 0;
            int regularNumbersCounter = 0;
            int hotNumbersCounter = 0;

            for (int j = 0; j < iResultsToConsider; j++)
            {
                for (int i = 0; i < iLotteryResults[j]._Numbers.Length; i++)
                {
                    int currNumber = iLotteryResults[j]._Numbers[i];
                    if (_HotNumbers.Contains(currNumber))
                    {
                        hotNumbersCounter++;
                    }
                    else if (_RegularNumbers.Contains(currNumber))
                    {
                        regularNumbersCounter++;
                    }
                    else
                    {
                        coldNumbersCounter++;
                    }
                }

                //cold regular hot
                using (StreamWriter writer = new StreamWriter(iFilename, true))
                {
                    string line = string.Format("{0},{1},{2}", coldNumbersCounter, regularNumbersCounter, hotNumbersCounter);
                    writer.WriteLine(line);
                }

                coldNumbersCounter = 0;
                hotNumbersCounter = 0;
                regularNumbersCounter = 0;
            }
        }

        public void GetNumbersDispersionPatterns(List<int[]> iLotteryResults, int iResultsToConsider, string iFilename)
        {
            int coldNumbersCounter = 0;
            int regularNumbersCounter = 0;
            int hotNumbersCounter = 0;

            for (int j = 0; j < iResultsToConsider; j++)
            {
                for (int i = 0; i < iLotteryResults[j].Length; i++)
                {
                    int currNumber = iLotteryResults[j][i];
                    if (_HotNumbers.Contains(currNumber))
                    {
                        hotNumbersCounter++;
                    }
                    else if (_RegularNumbers.Contains(currNumber))
                    {
                        regularNumbersCounter++;
                    }
                    else
                    {
                        coldNumbersCounter++;
                    }
                }

                //cold regular hot
                using (StreamWriter writer = new StreamWriter(iFilename, true))
                {
                    string line = string.Format("{0},{1},{2}", coldNumbersCounter, regularNumbersCounter, hotNumbersCounter);
                    writer.WriteLine(line);
                }

                coldNumbersCounter = 0;
                hotNumbersCounter = 0;
                regularNumbersCounter = 0;
            }
        }

        public int[] CalculateNumbersDispersion(List<LotteryWinningResult> iLotteryResults, int iResultsToConsider)
        {
            int[] numbersDispersion = new int[38];

            for (int j = 0; j < iResultsToConsider; j++ )
            {
                for (int i = 0; i < iLotteryResults[j]._Numbers.Length; i++)
                {
                    numbersDispersion[iLotteryResults[j]._Numbers[i]]++;
                }
            }

            return numbersDispersion;
        }

        private void ReadLotteryOfficialResultFile(string iFilepath)
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
                        int[] numbers = new int[6];

                        numbers[0] = int.Parse(lineSplit[2]);
                        numbers[1] = int.Parse(lineSplit[3]);
                        numbers[2] = int.Parse(lineSplit[4]);
                        numbers[3] = int.Parse(lineSplit[5]);
                        numbers[4] = int.Parse(lineSplit[6]);
                        numbers[5] = int.Parse(lineSplit[7]);

                        DateTime currDate;
                        bool isParsed = DateTime.TryParseExact(lineSplit[1], "dd/MM/yy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                        if (!isParsed)
                        {
                            isParsed = DateTime.TryParseExact(lineSplit[1], "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                            if (!isParsed)
                            {
                                isParsed = DateTime.TryParseExact(lineSplit[1], "dd/M/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                if (!isParsed)
                                {
                                    isParsed = DateTime.TryParseExact(lineSplit[1], "d/M/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                    if (!isParsed)
                                    {
                                        isParsed = DateTime.TryParseExact(lineSplit[1], "d/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                        if (!isParsed)
                                        {
                                            isParsed = DateTime.TryParseExact(lineSplit[1], "d/M/yy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                            if (!isParsed)
                                            {
                                                isParsed = DateTime.TryParseExact(lineSplit[1], "dd/M/yy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                                if (!isParsed)
                                                {
                                                    isParsed = DateTime.TryParseExact(lineSplit[1], "d/MM/yy", new CultureInfo("en-US"), DateTimeStyles.None, out currDate);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        

                        //0 - ID, 1 - Date, 2-7 numbers, 8 string number
                        _LotteryHistoricResults.Add(new LotteryWinningResult() {
                                                     _LotteryRaffleID = lineSplit[0],
                                                     _LotteryDate = currDate,
                                                     _Numbers = numbers,
                                                     _StrongNumber = int.Parse(lineSplit[8])});
                        
                        i++;
                    }

                    foreach (LotteryWinningResult winningResult in _LotteryHistoricResults)
                    {
                        List<int> numbersHit = new List<int>();
                        winningResult._HitCount = GetHitCountForTable(winningResult._Numbers, GetNumberOfOfficialCombinationsSinceDate(new DateTime(2009, 2, 28)), ref numbersHit);
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("Path {0} doesn't exist", Path.GetDirectoryName(iFilepath)));
            }
        }

        public List<int[]> GetOfficialCombinations(int iNumCombinations)
        {
            List<int[]> combinations = new List<int[]>();
            int wantedCombinatios = iNumCombinations;

            if (wantedCombinatios > _LotteryHistoricResults.Count)
            {
                wantedCombinatios = _LotteryHistoricResults.Count;
            }

            for (int i = 0; i < wantedCombinatios; i++)
            {
                combinations.Add(_LotteryHistoricResults[i]._Numbers);
            }

            return combinations;
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

        private bool IsContainsNumber(int iNumber, List<int> iNumbers)
        {
            bool isContains = false;

            if (iNumbers.Contains(iNumber))
            {
                isContains = true;
            }

            return isContains;
        }

        private int[] GetCommonessForSpecificNumber(int iNumber)
        {
            return _NumbersCommoness[iNumber];
        }

        private List<NumberCommoness> GetSortedCommonessForSpecificNumber(int iNumber, bool iReverse)
        {
            int[] commoness = GetCommonessForSpecificNumber(iNumber);
            List<NumberCommoness> sortedCommoness = new List<NumberCommoness>();

            for (int i = 1; i < commoness.Length; i++)
            {
                sortedCommoness.Add(new NumberCommoness() { Number = i, Commoness = commoness[i] });
            }

            sortedCommoness = sortedCommoness.OrderBy(plt => plt.Commoness).ToList<NumberCommoness>();

            if (iReverse)
            {
                sortedCommoness.Reverse();
            }

            return sortedCommoness;
        }

        private int[][] GetAllNumbersCommoness(string iFilename)
        {
            List<PartialLotteryTable> t = ReadPartialCombinationsFile(iFilename, false);

            int[][] combos2 = new int[38][];

            foreach (PartialLotteryTable tuple in t)
            {
                if (combos2[tuple.Numbers[0]] == null)
                {
                    combos2[tuple.Numbers[0]] = new int[38];
                }
                combos2[tuple.Numbers[0]][tuple.Numbers[1]] = tuple.Commonness;

                if (combos2[tuple.Numbers[1]] == null)
                {
                    combos2[tuple.Numbers[1]] = new int[38];
                }
                combos2[tuple.Numbers[1]][tuple.Numbers[0]] = tuple.Commonness;
            }

            return combos2;
        }

        public int GetHitCountForTable(int[] table, int iNumTablesToConsider, ref List<int> iNumbersHit)
        {
            int counter = 0;
            for ( int i =0; i < iNumTablesToConsider; i++)
            {
                int hitCount = Compare6NumbersTables(table, _LotteryHistoricResults[i]._Numbers, ref iNumbersHit);
                if (hitCount > 2)
                {
                    counter += 1;
                }
            }

            return counter;
        }

        private List<NumberCommoness> SetRank(int iNumber)
        {
            List<NumberCommoness> numberCommoness = GetSortedCommonessForSpecificNumber(iNumber, true);
            int rankCounter = 0;
            int lastNumberCommoness = 0;

            foreach (NumberCommoness number in numberCommoness)
            {
                if (number.Commoness != lastNumberCommoness)
                {
                    rankCounter++;
                }

                lastNumberCommoness = number.Commoness;
            }

            lastNumberCommoness = numberCommoness[0].Commoness;
            foreach (NumberCommoness number in numberCommoness)
            {
                if (number.Commoness != lastNumberCommoness)
                {
                    rankCounter--;
                    number.Rank = rankCounter;
                }
                else
                {
                    number.Rank = rankCounter;
                }

                lastNumberCommoness = number.Commoness;
            }

            return numberCommoness;
        }

        public void GetTableStatistics(int[] iTable, List<List<NumberCommoness>> iCommonessList, string iFilename)
        {
            for (int i = 0; i < iTable.Length; i++)
            {
                if (i > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        NumberCommoness currNumber = iCommonessList[iTable[i] - 1].Find(x => x.Number == iTable[j]);

                        //write data to file
                        string line = string.Format("{0},{1},{2},{3}", iTable[i], iTable[j], currNumber.Commoness, currNumber.Rank);
                        using (StreamWriter writer = new StreamWriter(iFilename, true))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                for (int j = i + 1; j < iTable.Length; j++)
                {
                    NumberCommoness currNumber = iCommonessList[iTable[i] - 1].Find(x => x.Number == iTable[j]);

                    //write data to file
                    string line = string.Format("{0},{1},{2},{3}", iTable[i], iTable[j], currNumber.Commoness, currNumber.Rank);
                    using (StreamWriter writer = new StreamWriter(iFilename, true))
                    {
                        writer.WriteLine(line);
                    }
                }
                
            }
        }

        public void PrintCommonessListToFile(List<List<NumberCommoness>> iCommonessList, string iFilename)
        {
            for (int i = 0; i < iCommonessList.Count; i++)
            {
                for (int j = 0; j < iCommonessList[i].Count; j++)
                {
                    string line = string.Format("{0},{1},{2},{3},{4}", i + 1, iCommonessList[i][j].Number, iCommonessList[i][j].Commoness, iCommonessList[i][j].Rank, iCommonessList[i][0].Rank);
                    using (StreamWriter writer = new StreamWriter(iFilename, true))
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }

        public void PrintTableDataToFile(int[] iTable, List<List<NumberCommoness>> iCommonessList, string iFilename)
        {
            string numbersBundle = string.Format("{0};{1};{2};{3};{4};{5}", iTable[0], iTable[1], iTable[2], iTable[3], iTable[4], iTable[5]);
            NumberCommoness num1 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[1]);
            NumberCommoness num2 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[2]);
            NumberCommoness num3 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[3]);
            NumberCommoness num4 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[4]);
            NumberCommoness num5 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[5]);

            List<NumberCommoness> sortedNumbersOnRank = new List<NumberCommoness>();
            sortedNumbersOnRank.Add(num1);
            sortedNumbersOnRank.Add(num2);
            sortedNumbersOnRank.Add(num3);
            sortedNumbersOnRank.Add(num4);
            sortedNumbersOnRank.Add(num5);

            sortedNumbersOnRank = sortedNumbersOnRank.OrderBy(x => x.Rank).ToList();

            string line = string.Format("{0},{1},{2},{3},{4},{5},{6},sorted on rank,{7},{8},{9},{10},{11}", iTable[0],numbersBundle , num1.Rank, num2.Rank, num3.Rank, num4.Rank, num5.Rank,
                                                                                    sortedNumbersOnRank[0].Rank,
                                                                                    sortedNumbersOnRank[1].Rank,
                                                                                    sortedNumbersOnRank[2].Rank,
                                                                                    sortedNumbersOnRank[3].Rank,
                                                                                    sortedNumbersOnRank[4].Rank);
            using (StreamWriter writer = new StreamWriter(iFilename, true))
            {
                writer.WriteLine(line);
            }
        }

        public void PrintWinningTableDataToFile(ChosenLotteryTable iTable, string iFilename)
        {
            string numbersBundle = string.Format("{0};{1};{2};{3};{4};{5}", iTable.Numbers[0], iTable.Numbers[1], iTable.Numbers[2], iTable.Numbers[3], iTable.Numbers[4], iTable.Numbers[5]);

            string line = string.Format("{0}, ,{1}, ,{2},{3},{4},{5},{6}, ,{7}", iTable.Leading, numbersBundle,
                                                                                    iTable.Ranks[0],
                                                                                    iTable.Ranks[1],
                                                                                    iTable.Ranks[2],
                                                                                    iTable.Ranks[3],
                                                                                    iTable.Ranks[4],
                                                                                    iTable.HitCount);
            using (StreamWriter writer = new StreamWriter(iFilename, true))
            {
                writer.WriteLine(line);
            }
        }

        public ChosenLotteryTable GetWinningTableData(DateTime iSince, int[] iTable, List<List<NumberCommoness>> iCommonessList)
        {
            ChosenLotteryTable winningLotteryTable = new ChosenLotteryTable();

            //string numbersBundle = string.Format("{0};{1};{2};{3};{4};{5}", iTable[0], iTable[1], iTable[2], iTable[3], iTable[4], iTable[5]);
            NumberCommoness num1 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[1]);
            NumberCommoness num2 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[2]);
            NumberCommoness num3 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[3]);
            NumberCommoness num4 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[4]);
            NumberCommoness num5 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[5]);

            winningLotteryTable.Leading = iTable[0];

            List<NumberCommoness> sortedNumbersOnRank = new List<NumberCommoness>();
            sortedNumbersOnRank.Add(num1);
            sortedNumbersOnRank.Add(num2);
            sortedNumbersOnRank.Add(num3);
            sortedNumbersOnRank.Add(num4);
            sortedNumbersOnRank.Add(num5);

            sortedNumbersOnRank = sortedNumbersOnRank.OrderBy(x => x.Rank).ToList();

            winningLotteryTable.Numbers = iTable;
            winningLotteryTable.Ranks = new int[]{sortedNumbersOnRank[0].Rank, sortedNumbersOnRank[1].Rank, sortedNumbersOnRank[2].Rank,
                                                 sortedNumbersOnRank[3].Rank, sortedNumbersOnRank[4].Rank};

            return winningLotteryTable;
        }
        
        public List<int[]> GetCombinationsAccordingToRanks(int[] iTable, List<List<NumberCommoness>> iCommonessList)
        {
            List<int[]> tables = new List<int[]>();

            //string numbersBundle = string.Format("{0};{1};{2};{3};{4};{5}", iTable[0], iTable[1], iTable[2], iTable[3], iTable[4], iTable[5]);
            NumberCommoness num1 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[1]);
            NumberCommoness num2 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[2]);
            NumberCommoness num3 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[3]);
            NumberCommoness num4 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[4]);
            NumberCommoness num5 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[5]);

            List<NumberCommoness> sortedNumbersOnRank = new List<NumberCommoness>();
            sortedNumbersOnRank.Add(num1);
            sortedNumbersOnRank.Add(num2);
            sortedNumbersOnRank.Add(num3);
            sortedNumbersOnRank.Add(num4);
            sortedNumbersOnRank.Add(num5);

            try
            {
                sortedNumbersOnRank = sortedNumbersOnRank.OrderBy(x => x.Rank).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            List<int> num1Possibilties = GetNumbersWithSpecificRank(iTable[0], num1.Rank);
            List<int> num2Possibilties = GetNumbersWithSpecificRank(iTable[0], num2.Rank);
            List<int> num3Possibilties = GetNumbersWithSpecificRank(iTable[0], num3.Rank);
            List<int> num4Possibilties = GetNumbersWithSpecificRank(iTable[0], num4.Rank);
            List<int> num5Possibilties = GetNumbersWithSpecificRank(iTable[0], num5.Rank);
            //int totalPossibilities = num1Possibilties.Count * num2Possibilties.Count * num3Possibilties.Count * num4Possibilties.Count * num5Possibilties.Count;

            for (int i = 0; i < num1Possibilties.Count; i++)
            {
                for (int j = 0; j < num2Possibilties.Count; j++)
                {
                    for (int k = 0; k < num3Possibilties.Count; k++)
                    {
                        for (int l = 0; l < num4Possibilties.Count; l++)
                        {
                            for (int t = 0; t < num5Possibilties.Count; t++)
                            {
                                int[] table = new int[] { iTable[0], num1Possibilties[i], num2Possibilties[j], num3Possibilties[k], num4Possibilties[l], num5Possibilties[t] };

                                if (table.Distinct().Count() == 6)
                                {
                                    tables.Add(table.OrderBy(x => x).ToArray<int>());  
                                }
                            }
                        }
                    }
                }
            }

            return tables;
        }

        public List<ChosenLotteryTable> GetCombinationsAccordingToRanks2(int[] iTable, List<List<NumberCommoness>> iCommonessList)
        {
            List<ChosenLotteryTable> tables = new List<ChosenLotteryTable>();

            //string numbersBundle = string.Format("{0};{1};{2};{3};{4};{5}", iTable[0], iTable[1], iTable[2], iTable[3], iTable[4], iTable[5]);
            NumberCommoness num1 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[1]);
            NumberCommoness num2 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[2]);
            NumberCommoness num3 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[3]);
            NumberCommoness num4 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[4]);
            NumberCommoness num5 = iCommonessList[iTable[0] - 1].Find(x => x.Number == iTable[5]);

            List<NumberCommoness> sortedNumbersOnRank = new List<NumberCommoness>();
            sortedNumbersOnRank.Add(num1);
            sortedNumbersOnRank.Add(num2);
            sortedNumbersOnRank.Add(num3);
            sortedNumbersOnRank.Add(num4);
            sortedNumbersOnRank.Add(num5);

            try
            {
                sortedNumbersOnRank = sortedNumbersOnRank.OrderBy(x => x.Rank).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            List<int> num1Possibilties = GetNumbersWithSpecificRank(iTable[0], num1.Rank);
            List<int> num2Possibilties = GetNumbersWithSpecificRank(iTable[0], num2.Rank);
            List<int> num3Possibilties = GetNumbersWithSpecificRank(iTable[0], num3.Rank);
            List<int> num4Possibilties = GetNumbersWithSpecificRank(iTable[0], num4.Rank);
            List<int> num5Possibilties = GetNumbersWithSpecificRank(iTable[0], num5.Rank);
            //int totalPossibilities = num1Possibilties.Count * num2Possibilties.Count * num3Possibilties.Count * num4Possibilties.Count * num5Possibilties.Count;

            for (int i = 0; i < num1Possibilties.Count; i++)
            {
                for (int j = 0; j < num2Possibilties.Count; j++)
                {
                    for (int k = 0; k < num3Possibilties.Count; k++)
                    {
                        for (int l = 0; l < num4Possibilties.Count; l++)
                        {
                            for (int t = 0; t < num5Possibilties.Count; t++)
                            {
                                int[] table = new int[] { iTable[0], num1Possibilties[i], num2Possibilties[j], num3Possibilties[k], num4Possibilties[l], num5Possibilties[t] };

                                if (table.Distinct().Count() == 6)
                                {
                                    tables.Add( new ChosenLotteryTable(){ Numbers = table.OrderBy(x => x).ToArray<int>(),
                                                                          Ranks = new int[]{num1.Rank, num2.Rank, num3.Rank, num4.Rank, num5.Rank }.OrderBy(x => x).ToArray(),
                                                                          Leading = iTable[0]});
                                }
                            }
                        }
                    }
                }
            }

            return tables;
        }

        public List<int> GetNumbersWithSpecificRank(int iNumber, int iRank)
        {
            List<int> numbers = new List<int>();

            IEnumerable<NumberCommoness> byRank = _RankedNumbers[iNumber - 1].Where(x => x.Rank == iRank);

            foreach(NumberCommoness number in byRank)
            {
                numbers.Add(number.Number);
            }

            return numbers;
        }

        public void GetAllLotteryTablesCombinations(string iFilename)
        {
            List<ChosenLotteryTable> combinations = new List<ChosenLotteryTable>();
            List<ChosenLotteryTable> winningCombinations = new List<ChosenLotteryTable>();

            //1.Create the commoness file
            Find2CombinationsInside6Combinations(GetOfficialCombinationsByDate(new DateTime(2009, 2, 28)), 38, "2CombinationsOf6Hits.csv", false);

            //2.Read the commoness file and set the ranks
            ReadCommonessFile("2CombinationsOf6Hits.csv");
            //lotteryEngine.PrintCommonessListToFile(lotteryEngine.RankedNumbers, "CurrentCommoness.csv");

            //3.Create winning tables list
            List<int[]> winningResults = GetOfficialCombinationsByDate(new DateTime(2009, 2, 28));
            foreach (int[] winningResult in winningResults)
            {
                //PrintTableDataToFile(winningResult, lotteryEngine.RankedNumbers, "winningTablesData.csv");
                winningCombinations.Add(GetWinningTableData(new DateTime(2009, 2, 28), winningResult, RankedNumbers));
                combinations.AddRange(GetCombinationsAccordingToRanks2(winningResult, RankedNumbers).Distinct(new DistinctChosenLotteryTableComparer()));
            }

            //combinations = combinations.Distinct(new DistinctChosenLotteryTableComparer()).ToList();

            List<ChosenLotteryTable> chosenTables = new List<ChosenLotteryTable>();
            foreach (ChosenLotteryTable winningTable in winningCombinations)
            {
                for (int j = 0; j < combinations.Count; j++)
                {
                    if (CompareIntArray(winningTable.Ranks, combinations[j].Ranks))
                    {
                        List<int> numbersHit = new List<int>();
                        int hitCount = GetHitCountForTable(combinations[j].Numbers, winningCombinations.Count, ref numbersHit);
                        combinations[j].HitCount = hitCount;
                        chosenTables.Add(combinations[j]);

                        using (StreamWriter writer = new StreamWriter(iFilename, true))
                        {
                            string table = string.Format("{0};{1};{2};{3};{4};{5}", combinations[j].Numbers[0], combinations[j].Numbers[1],
                                                                                    combinations[j].Numbers[2], combinations[j].Numbers[3],
                                                                                    combinations[j].Numbers[4], combinations[j].Numbers[5]);
                            string line = string.Format("{0}, ,{1}, ,{2},{3},{4},{5},{6}, ,{7}", combinations[j].Leading, table, combinations[j].Ranks[0],
                                                                                                                 combinations[j].Ranks[1],
                                                                                                                 combinations[j].Ranks[2],
                                                                                                                 combinations[j].Ranks[3],
                                                                                                                 combinations[j].Ranks[4],
                                                                                                                 combinations[j].HitCount);

                            writer.WriteLine(line);
                        }
                    }
                }
            }
        }

        public void GetAllLotteryTablesCombinations2(string iFilename, bool iWriteWinningTablesDataToFile, string iWinningTablesFilename)
        {
            List<ChosenLotteryTable> combinations = new List<ChosenLotteryTable>();
            List<ChosenLotteryTable> winningCombinations = new List<ChosenLotteryTable>();

            //1.Create the commoness file
            Find2CombinationsInside6Combinations(GetOfficialCombinationsByDate(new DateTime(2009, 2, 28)), 38, "2CombinationsOf6Hits.csv", false);

            //2.Read the commoness file and set the ranks
            ReadCommonessFile("2CombinationsOf6Hits.csv");
            //lotteryEngine.PrintCommonessListToFile(lotteryEngine.RankedNumbers, "CurrentCommoness.csv");

            //3.Create winning tables list
            List<int[]> winningResults = GetOfficialCombinationsByDate(new DateTime(2009, 2, 28));
            foreach (int[] winningResult in winningResults)
            {
                ChosenLotteryTable currTable = GetWinningTableData(new DateTime(2009, 2, 28), winningResult, RankedNumbers);
                winningCombinations.Add(currTable);
                if (iWriteWinningTablesDataToFile)
                {
                    LotteryWinningResult tmp = GetOfficialWinningResult(winningResult, GetNumberOfOfficialCombinationsSinceDate(new DateTime(2009, 2, 28)));
                    currTable.HitCount = tmp._HitCount;
                    PrintWinningTableDataToFile(currTable, iWinningTablesFilename);
                }
                combinations.AddRange(GetCombinationsAccordingToRanks2(winningResult, RankedNumbers).Distinct(new DistinctChosenLotteryTableComparer()));
            }

            //combinations = combinations.Distinct(new DistinctChosenLotteryTableComparer()).ToList();

            List<ChosenLotteryTable> chosenTables = new List<ChosenLotteryTable>();
            foreach (ChosenLotteryTable winningTable in winningCombinations)
            {
                for (int j = 0; j < combinations.Count; j++)
                {
                    if (CompareIntArray(winningTable.Ranks, combinations[j].Ranks))
                    {
                        List<int> numbersHit = new List<int>();
                        int hitCount = GetHitCountForTable(combinations[j].Numbers, winningCombinations.Count, ref numbersHit);
                        combinations[j].HitCount = hitCount;
                        chosenTables.Add(combinations[j]);
                    }
                }
            }

            //Now that we have the hit count for the winning combinations, we need to go over all found combinations and see if they match
            //the winning combinations in terms of ranks and hit count, if so add to the list of chosen combinations
            List<ChosenLotteryTable> chosenCombinations = new List<ChosenLotteryTable>();

            foreach (ChosenLotteryTable table in combinations)
            {
                for (int i = 0; i < winningCombinations.Count; i++)
                {
                    if (table.HitCount == _LotteryHistoricResults[i]._HitCount &&
                        CompareIntArray(table.Ranks, winningCombinations[i].Ranks) &&
                        !CompareIntArray(table.Numbers, winningCombinations[i].Numbers)/*match ranks && hit count*/)
                    {
                        //chosenCombinations.Add(table);

                        //need to make sure that 3 hot numbers don't appear in the table together

                        if (HotNumbersCountInTable(table.Numbers) < 3 && table.Numbers[0] == table.Leading)
                        {
                            using (StreamWriter writer = new StreamWriter(iFilename, true))
                            {
                                string strTable = string.Format("{0};{1};{2};{3};{4};{5}", table.Numbers[0], table.Numbers[1],
                                                                                        table.Numbers[2], table.Numbers[3],
                                                                                        table.Numbers[4], table.Numbers[5]);
                                string line = string.Format("{0}, ,{1}, ,{2},{3},{4},{5},{6}, ,{7}", table.Leading, strTable, table.Ranks[0],
                                                                                                                     table.Ranks[1],
                                                                                                                     table.Ranks[2],
                                                                                                                     table.Ranks[3],
                                                                                                                     table.Ranks[4],
                                                                                                                     table.HitCount);

                                writer.WriteLine(line);
                            } 
                        }
                    }
                }
            }
        }

        public void GetAllLotteryTablesCombinations3(string iGeneratedTablesFilename, bool iWriteGeneratedTables, int iWantedCombinations, Dictionary<int, int> iWantedDispersion, bool iWriteWinningTablesDataToFile, string iWinningTablesFilename, string iChosenTablesFilename)
        {
            List<ChosenLotteryTable> combinations = new List<ChosenLotteryTable>();
            List<ChosenLotteryTable> winningCombinations = new List<ChosenLotteryTable>();

            //1.Create the commoness file
            Find2CombinationsInside6Combinations(GetOfficialCombinationsByDate(new DateTime(2009, 2, 28)), 38, "2CombinationsOf6Hits.csv", false);

            //2.Read the commoness file and set the ranks
            ReadCommonessFile("2CombinationsOf6Hits.csv");
            //lotteryEngine.PrintCommonessListToFile(lotteryEngine.RankedNumbers, "CurrentCommoness.csv");

            //3.Create winning tables list
            List<int[]> winningResults = GetOfficialCombinationsByDate(new DateTime(2009, 2, 28));
            foreach (int[] winningResult in winningResults)
            {
                ChosenLotteryTable currTable = GetWinningTableData(new DateTime(2009, 2, 28), winningResult, RankedNumbers);
                winningCombinations.Add(currTable);

                LotteryWinningResult tmp = GetOfficialWinningResult(winningResult, GetNumberOfOfficialCombinationsSinceDate(new DateTime(2009, 2, 28)));
                currTable.HitCount = tmp._HitCount;

                if (iWriteWinningTablesDataToFile)
                {
                    PrintWinningTableDataToFile(currTable, iWinningTablesFilename);
                }
                combinations.AddRange(GetCombinationsAccordingToRanks2(winningResult, RankedNumbers).Distinct(new DistinctChosenLotteryTableComparer()));
            }

            List<ChosenLotteryTable> chosenTables = new List<ChosenLotteryTable>();
            foreach (ChosenLotteryTable winningTable in winningCombinations)
            {
                for (int j = 0; j < combinations.Count; j++)
                {
                    if (CompareIntArray(winningTable.Ranks, combinations[j].Ranks))
                    {
                        List<int> numbersHit = new List<int>();
                        int hitCount = GetHitCountForTable(combinations[j].Numbers, winningCombinations.Count, ref numbersHit);
                        combinations[j].HitCount = hitCount;
                        chosenTables.Add(combinations[j]);

                        if (iWriteGeneratedTables)
                        {
                            using (StreamWriter writer = new StreamWriter(iGeneratedTablesFilename, true))
                            {
                                string strTable = string.Format("{0};{1};{2};{3};{4};{5}", combinations[j].Numbers[0], combinations[j].Numbers[1],
                                                                                        combinations[j].Numbers[2], combinations[j].Numbers[3],
                                                                                        combinations[j].Numbers[4], combinations[j].Numbers[5]);
                                string line = string.Format("{0}, ,{1}, ,{2}, ,{3},{4},{5},{6},{7}, ,{8}", combinations[j].Leading, strTable, combinations[j].StrongNumber, combinations[j].Ranks[0],
                                                                                                                     combinations[j].Ranks[1],
                                                                                                                     combinations[j].Ranks[2],
                                                                                                                     combinations[j].Ranks[3],
                                                                                                                     combinations[j].Ranks[4],
                                                                                                                     combinations[j].HitCount);

                                writer.WriteLine(line);
                            }           
                        }
                    }
                }
            }

            //Now that we have the hit count for the winning combinations, we need to go over all found combinations and see if they match
            //the winning combinations in terms of ranks and hit count, if so add to the list of chosen combinations
            List<ChosenLotteryTable> chosenCombinations = new List<ChosenLotteryTable>();
            int currStrongNumber = 1;
            foreach (ChosenLotteryTable table in combinations)
            {
                for (int i = 0; i < winningCombinations.Count; i++)
                {
                    if (table.HitCount == _LotteryHistoricResults[i]._HitCount &&
                        CompareIntArray(table.Ranks, winningCombinations[i].Ranks) &&
                        !CompareIntArray(table.Numbers, winningCombinations[i].Numbers)/*match ranks && hit count*/)
                    {
                        table.StrongNumber = currStrongNumber;
                        chosenCombinations.Add(table);

                        currStrongNumber++;
                        if (currStrongNumber == 8)
                        {
                            currStrongNumber = 1;
                        }
                    }
                }
            }

            //Choose from chosen combinations according to wanted dispersion
            List<ChosenLotteryTable> chosenForSending = new List<ChosenLotteryTable>();
            foreach (KeyValuePair<int, int> pair in iWantedDispersion)
            {
                chosenForSending.AddRange(ChooseCombinationsForSpecificNumber(pair.Key, (int)((double)pair.Value / 100 * iWantedCombinations), winningCombinations.Where(x => x.Leading == pair.Key).ToList(), chosenCombinations));
            }

            //Write ChosenForSending to file
            foreach (ChosenLotteryTable result in chosenForSending)
            {
                using (StreamWriter writer = new StreamWriter(iChosenTablesFilename, true))
                {
                    string strTable = string.Format("{0};{1};{2};{3};{4};{5}", result.Numbers[0], result.Numbers[1],
                                                                            result.Numbers[2], result.Numbers[3],
                                                                            result.Numbers[4], result.Numbers[5]);
                    string line = string.Format("{0}, ,{1}, ,{2}, ,{3},{4},{5},{6},{7}, ,{8}", result.Leading, strTable, result.StrongNumber, result.Ranks[0],
                                                                                                         result.Ranks[1],
                                                                                                         result.Ranks[2],
                                                                                                         result.Ranks[3],
                                                                                                         result.Ranks[4],
                                                                                                         result.HitCount);

                    writer.WriteLine(line);
                }
            }
        }

        public void GetLotteryTablesCombinationsAimFor3Match(string iFilename, int iWantedCombinations, Dictionary<int, int> iWantedDispersion, bool iWriteWinningTablesDataToFile, string iChosenTablesFilename)
        {
            List<ChosenLotteryTable> combinations = new List<ChosenLotteryTable>();
            List<ChosenLotteryTable> winningCombinations = new List<ChosenLotteryTable>();

            //1.Create the commoness file
            Find2CombinationsInside6Combinations(GetOfficialCombinationsByDate(new DateTime(2009, 2, 28)), 38, "2CombinationsOf6Hits.csv", false);

            //2.Read the commoness file and set the ranks
            ReadCommonessFile("2CombinationsOf6Hits.csv");
            //lotteryEngine.PrintCommonessListToFile(lotteryEngine.RankedNumbers, "CurrentCommoness.csv");

            //3.Create winning tables list
            List<int[]> winningResults = GetOfficialCombinationsByDate(new DateTime(2009, 2, 28));
            foreach (int[] winningResult in winningResults)
            {
                ChosenLotteryTable currTable = GetWinningTableData(new DateTime(2009, 2, 28), winningResult, RankedNumbers);
                winningCombinations.Add(currTable);
                //if (iWriteWinningTablesDataToFile)
                //{
                LotteryWinningResult tmp = GetOfficialWinningResult(winningResult, GetNumberOfOfficialCombinationsSinceDate(new DateTime(2009, 2, 28)));
                currTable.HitCount = tmp._HitCount;
                //    PrintWinningTableDataToFile(currTable, iChosenTablesFilename);
                //}
                combinations.AddRange(GetCombinationsAccordingToRanks2(winningResult, RankedNumbers).Distinct(new DistinctChosenLotteryTableComparer()));
            }

            List<ChosenLotteryTable> chosenTables = new List<ChosenLotteryTable>();
            foreach (ChosenLotteryTable winningTable in winningCombinations)
            {
                for (int j = 0; j < combinations.Count; j++)
                {
                    if (CompareIntArray(winningTable.Ranks, combinations[j].Ranks))
                    {
                        List<int> numbersHit = new List<int>();
                        int hitCount = GetHitCountForTable(combinations[j].Numbers, winningCombinations.Count, ref numbersHit);
                        combinations[j].HitCount = hitCount;
                        chosenTables.Add(combinations[j]);
                    }
                }
            }

            chosenTables = chosenTables.OrderBy(x => x.HitCount).Reverse().ToList();

            //Choose from chosen combinations according to wanted dispersion
            List<ChosenLotteryTable> chosenForSending = new List<ChosenLotteryTable>();
            foreach (KeyValuePair<int, int> pair in iWantedDispersion)
            {

                chosenForSending.AddRange(chosenTables.Take((int)((double)pair.Value / 100 * iWantedCombinations)));
                //chosenForSending.AddRange(ChooseCombinationsForSpecificNumberAimFor3(pair.Key, (int)((double)pair.Value / 100 * iWantedCombinations), winningCombinations.Where(x => x.Leading == pair.Key).ToList(), chosenCombinations));
            }

            //Write ChosenForSending to file
            foreach (ChosenLotteryTable result in chosenForSending)
            {
                using (StreamWriter writer = new StreamWriter(iChosenTablesFilename, true))
                {
                    string strTable = string.Format("{0};{1};{2};{3};{4};{5}", result.Numbers[0], result.Numbers[1],
                                                                            result.Numbers[2], result.Numbers[3],
                                                                            result.Numbers[4], result.Numbers[5]);
                    string line = string.Format("{0}, ,{1}, ,{2}, ,{3},{4},{5},{6},{7}, ,{8}", result.Leading, strTable, result.StrongNumber, result.Ranks[0],
                                                                                                         result.Ranks[1],
                                                                                                         result.Ranks[2],
                                                                                                         result.Ranks[3],
                                                                                                         result.Ranks[4],
                                                                                                         result.HitCount);

                    writer.WriteLine(line);
                }
            }
        }

        private List<ChosenLotteryTable> ChooseCombinationsForSpecificNumber(int iLeadingNumber, int iNumCombinations, List<ChosenLotteryTable> iWinningCombinations, List<ChosenLotteryTable> iCombinationsToChooseFrom)
        {
            List<ChosenLotteryTable> combinations = new List<ChosenLotteryTable>();
            int counter = 0;

            IEnumerable<IGrouping<int, ChosenLotteryTable>> listGroupByHitCount = iWinningCombinations.GroupBy(x => x.HitCount);
            IEnumerable<IGrouping<int[], ChosenLotteryTable>> listGroupByRanks = iWinningCombinations.GroupBy(x => x.Ranks);

            //Ordering the groups grouped by hit count by value, so groups with the most common hit count will appear higher in the list
            List<KeyValuePair<int, int>> listOrderByHitCountValue = new List<KeyValuePair<int, int>>();
            foreach (IGrouping<int, ChosenLotteryTable> item in listGroupByHitCount)
            {
                listOrderByHitCountValue.Add(new KeyValuePair<int,int>(item.Key, item.Count()));
            }
            listOrderByHitCountValue = listOrderByHitCountValue.OrderBy(x => x.Value).Reverse().ToList();

            //Need to choose from both hit count and ranks

            for (int i = 0; i < listOrderByHitCountValue.Count; i++)
            {
                var res = iCombinationsToChooseFrom.Where(x => x.HitCount == listOrderByHitCountValue[i].Key && x.Leading == iLeadingNumber);

                if (counter + res.Count() <= iNumCombinations)
                {
                    combinations.AddRange((List<ChosenLotteryTable>)res.ToList());
                    counter += res.Count();
                }
                else
                {
                    combinations.AddRange((List<ChosenLotteryTable>)res.Take(iNumCombinations - counter).ToList());
                    break;
                }
            }

            return combinations;
        }

        public List<ChosenLotteryTable> ReadChosenCombinations(string iFilepath)
        {
            string path = string.Empty;
            List<ChosenLotteryTable> records = new List<ChosenLotteryTable>();

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
                    //reader.ReadLine(); //skipping first line


                    //Leading, ,numbers by;, ,strong , ,rank1, rank2, rank3, rank4, rank5, ,hitcount
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] lineSplit = line.Split(',');
                        int[] numbers = new int[6];
                        int[] ranks = new int[5];

                        int leading = int.Parse(lineSplit[0]);
                        string[] numbersSplit = lineSplit[2].Split(';');

                        numbers[0] = int.Parse(numbersSplit[0]);
                        numbers[1] = int.Parse(numbersSplit[1]);
                        numbers[2] = int.Parse(numbersSplit[2]);
                        numbers[3] = int.Parse(numbersSplit[3]);
                        numbers[4] = int.Parse(numbersSplit[4]);
                        numbers[5] = int.Parse(numbersSplit[5]);

                        ranks[0] = int.Parse(lineSplit[6]);
                        ranks[1] = int.Parse(lineSplit[7]);
                        ranks[2] = int.Parse(lineSplit[8]);
                        ranks[3] = int.Parse(lineSplit[9]);
                        ranks[4] = int.Parse(lineSplit[10]);

                        records.Add(new ChosenLotteryTable()
                        {
                            Leading = leading,
                            Numbers = numbers,
                            Ranks = ranks,
                            HitCount = int.Parse(lineSplit[12]),
                            StrongNumber = int.Parse(lineSplit[4])
                        });

                        i++;
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("Path {0} doesn't exist", Path.GetDirectoryName(iFilepath)));
            }

            return records;
        }

        public int[] CheckHitCountForChosenCombinations(int[] iTableToCompare, int iStrongNumber, List<ChosenLotteryTable> iChosenCombinations)
        {
            int[] hitCount = new int[11]; //0 1 2 3 4  5 6  7 8  9 10
                                          //      3 3+ 4 4+ 5 5+ 6 6+
            
            if (iTableToCompare != null)
            {
                foreach (ChosenLotteryTable chosenTable in iChosenCombinations)
                {
                    int hitCounter = Compare6NumbersTables(chosenTable.Numbers, iTableToCompare);

                    if (hitCounter > 2)
	                {
                        if (chosenTable.StrongNumber == iStrongNumber)
                        {
                            switch(hitCounter)
                            {
                                case 3:
                                    hitCount[hitCounter + 1]++;
                                    break;
                                case 4:
                                    hitCount[hitCounter + 2]++;
                                    break;
                                case 5:
                                    hitCount[hitCounter + 3]++;
                                    break;
                                case 6:
                                    hitCount[hitCounter + 4]++;
                                    break;
                            }
                        }
                        else
                        {
                            switch (hitCounter)
                            {
                                case 3:
                                    hitCount[hitCounter]++;
                                    break;
                                case 4:
                                    hitCount[hitCounter + 1]++;
                                    break;
                                case 5:
                                    hitCount[hitCounter + 2]++;
                                    break;
                                case 6:
                                    hitCount[hitCounter + 3]++;
                                    break;
                            }
                        }

	                }
                }
            }

            return hitCount;
        }

        /// <summary>
        /// This function searches the currently loaded official results
        /// </summary>
        /// <param name="iWinningResult"></param>
        /// <returns></returns>
        private LotteryWinningResult GetOfficialWinningResult(int[] iWinningResult, int iNumResultsToConsider)
        {
            LotteryWinningResult result = null;

            for (int i = 0; i < iNumResultsToConsider; i++)
            {
                if (CompareIntArray(iWinningResult, _LotteryHistoricResults[i]._Numbers))
                {
                    result = _LotteryHistoricResults[i];
                    break;
                }
            }

            return result;
        }

        private int HotNumbersCountInTable(int[] iTable)
        {
            int counter = 0;

            for (int j = 0; j < _HotNumbersKVP.Count; j++)
            {
                if (IsNumberExists(_HotNumbersKVP[j].Key, iTable))
                {
                    counter++;
                }
            }

            return counter;
        }

        private List<KeyValuePair<int, int>> PopulateHotNumbersList(List<int[]> iWinningResults, int iNumHotNumbers)
        {
            int[] winningResultsDispersion = new int[38];
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
            KeyValuePair<int, int> pair = new KeyValuePair<int,int>(-1,-1);
            for (int i = 1; i < iArray.Length; i++)
            {
                if (iArray[i] > pair.Value)
                {
                    pair = new KeyValuePair<int,int>(i, iArray[i]);
                }
            }

            return pair;
        }

        #region NotUsedMethods
        public void PopulateNumbersTables(int[] iNumberDispersion, int iHotCriteria, int iColdCriteria)
        {
            for (int i = 1; i < iNumberDispersion.Length; i++)
            {
                if (iNumberDispersion[i] <= iColdCriteria)
                {
                    _ColdNumbers.Add(i);
                }
                else if (iNumberDispersion[i] > iColdCriteria && iNumberDispersion[i] < iHotCriteria)
                {
                    _RegularNumbers.Add(i);
                }
                else if (iNumberDispersion[i] >= iHotCriteria)
                {
                    _HotNumbers.Add(i);
                }
            }
        }

        public void GetLotteryTables(int iNumberOfTables, string iWininingsFile, string iNumbersFile)
        {
            _WantedTables = iNumberOfTables;
            GenerateMultipleTables(iNumberOfTables);
            ReplaceRepetitionsWithStrongNumbers();
            CheckHitCount(iWininingsFile);

            if (iNumbersFile != null && iNumbersFile != string.Empty)
            {
                WriteTablesToFile(iNumbersFile);
            }

            _Batches.Clear();
        }

        public void GenerateMultipleTables(int iNumberOfTables)
        {
            int i = 0;
            while (i < iNumberOfTables)
            {
                LotteryTable lt = GenerateSingleTable();
                if (!IsTableExists(lt))
                {
                    _LotteryTables.Add(lt);
                    i++;
                }

                if (_LotteryTables.Count == 6)
                {

                    _Batches.Add(new List<LotteryTable>(_LotteryTables));
                    _LotteryTables.Clear();
                }
            }

        }

        private void ReplaceRepetitionsWithStrongNumbers()
        {
            ///starting from the 2nd batch, replace a non-strong number with a strong number,
            ///if that strong number is not part of the table already
            ///Keep in mind, that eventually, the strong numbers need to appear in a batch as many times as their factor

            if (_Batches.Count > 1)
            {
                _Repetitions = FindAllRepetitions();

                int i = 1;
                int j = 0;

                while (i < _Batches.Count)
                {
                    foreach (LotteryTable table in _Batches[i])
                    {
                        for (j = 0; j < table._Numbers.Length; j++)
                        {
                            if (!IsHotNumber(table._Numbers[j]))
                            {
                                int[] currBatchRepetitions = FindRepetitionsPerBatch(_Batches[i]);
                                NumberStatistics hotNumber = null;
                                bool isReachedMaxRepetitions = false;
                                List<int> selectedHotNumbers = new List<int>();

                                //Pick a hot number until a hot number with no sufficient repetitions is found or that hot number is not part of the table already
                                do
                                {
                                    isReachedMaxRepetitions = false;
                                    hotNumber = PickHotNumber();

                                    if (!selectedHotNumbers.Contains(hotNumber.Number))
                                    {
                                        selectedHotNumbers.Add(hotNumber.Number);
                                    }

                                    if (hotNumber.Factor == currBatchRepetitions[hotNumber.Number])
                                    {
                                        isReachedMaxRepetitions = true;
                                    }

                                    if (selectedHotNumbers.Count == _FactorHotNumbers.Count)
                                    {
                                        hotNumber = null;
                                        selectedHotNumbers.Clear();
                                        break;
                                    }
                                } while (isReachedMaxRepetitions || IsNumberExists(hotNumber.Number, table._Numbers));


                                //int idx = -1;
                                //for (int t = 0; t < _HotNumbers.Count; t++)
                                //{
                                //    if (_HotNumbers[t].Number == hotNumber.Number)
                                //    {
                                //        idx = t;
                                //        break;
                                //    }
                                //}


                                if (hotNumber != null)
                                {
                                    table._Numbers.SetValue(hotNumber.Number, j);
                                    table._Numbers = table._Numbers.OrderBy(k => k).ToArray();
                                }
                            }
                        }
                    }

                    i++;
                }
            }
            else
            {
                throw new Exception("Need more than one batch to work on...");
            }
        }

        private NumberStatistics PickHotNumber()
        {
            //need to generate a random number between 0 & count of hotnumber list - 1 to get a hot number
            int placeInList = _RandomNumbersGenerator.Next(0, _FactorHotNumbers.Count);

            return _FactorHotNumbers.ElementAt(placeInList);
        }

        private int[] FindRepetitionsPerBatch(List<LotteryTable> iLotteryTablesBatch)
        {
            int[] repetitions = new int[38];

            foreach (LotteryTable table in iLotteryTablesBatch)
            {
                foreach (int number in table._Numbers)
                {
                    repetitions[number]++;
                }
            }

            return repetitions;
        }

        public int[] FindAllRepetitions()
        {
            int i = 0;
            int j = 0;
            int[] numberStatistics = new int[38];

            while (i < _Batches.Count)
            {
                foreach (LotteryTable table in _Batches[i])
                {
                    foreach (int number in table._Numbers)
                    {
                        numberStatistics[number]++;
                    }
                }

                i++;
            }

            return numberStatistics;
        }

        public void WriteTablesToFile(string iFilepath)
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
                using (StreamWriter writer = new StreamWriter(iFilepath, true))
                {
                    writer.WriteLine("Number1, Number2, Number3, Number4, Number5, Number6, Strong Number");

                    int i = 0;
                    while (i < _Batches.Count)
                    {
                        foreach (LotteryTable table in _Batches[i])
                        {
                            string currTable = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                         table._Numbers[0],
                                                         table._Numbers[1],
                                                         table._Numbers[2],
                                                         table._Numbers[3],
                                                         table._Numbers[4],
                                                         table._Numbers[5],
                                                         table._StrongNumber);
                            writer.WriteLine(currTable);
                        }

                        i++;
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("Path {0} doesn't exist", Path.GetDirectoryName(iFilepath)));
            }
        }

        private bool IsTableExists(LotteryTable iLotteryTable)
        {
            bool isTableExists = false;

            //check in current batch
            foreach (LotteryTable table in _LotteryTables)
            {
                if (Compare(iLotteryTable, table) == true)
                {
                    isTableExists = true;
                    break;
                }
            }

            //check in all batches
            int i = 0;
            int j = 0;

            while (i < _Batches.Count)
            {
                foreach (LotteryTable table in _Batches[i])
                {
                    if (FullCompare(iLotteryTable, table) == true)
                    {
                        isTableExists = true;
                        break;
                    }
                }

                i++;
            }

            return isTableExists;
        }

        //Wrong compare - need to repair!!!
        private bool Compare(LotteryTable iLotteryTable1, LotteryTable iLotteryTable2)
        {
            bool isIdentical = false;

            foreach (int number1 in iLotteryTable1._Numbers)
            {
                foreach (int number2 in iLotteryTable2._Numbers)
                {
                    if (number1 == number2)//  && !IsNumberExists(number1, _HotNumbers))
                    {
                        isIdentical = true;
                        break;
                    }
                }

                if (isIdentical == true)
                {
                    break;
                }
            }

            return isIdentical;
        }

        private bool FullCompare(LotteryTable iLotteryTable1, LotteryTable iLotteryTable2)
        {
            int counter = 0;
            bool isIdentical = false;

            foreach (int number1 in iLotteryTable1._Numbers)
            {
                foreach (int number2 in iLotteryTable2._Numbers)
                {
                    if (number1 == number2)//  && !IsNumberExists(number1, _HotNumbers))
                    {
                        counter++;
                    }
                }
            }

            if (counter == 6)
            {
                isIdentical = true;
            }

            return isIdentical;
        }

        public LotteryTable GenerateSingleTable()
        {
            int[] numbers = new int[6] { 0, 0, 0, 0, 0, 0 };
            int strongNumber = 0;

            int i = 0;

            while (i < 6)
            {
                int currNumber = _RandomNumbersGenerator.Next(1, 38);

                if (!IsNumberExists(currNumber, numbers))
                {
                    numbers[i] = currNumber;
                    i++;
                }
            }

            strongNumber = _RandomNumbersGenerator.Next(1, 8);

            return new LotteryTable() { _Numbers = numbers.OrderBy(k => k).ToArray(), _StrongNumber = strongNumber };
        }

        private bool IsNumberExists(int iNumber, int[] iNumbers)
        {
            int i;
            bool isExists = false;

            for (i = 0; i < iNumbers.Length; i++)
            {
                if (iNumber == iNumbers[i])
                {
                    isExists = true;
                }
            }

            return isExists;
        }

        private bool IsHotNumber(int iNumberToCheck)
        {
            bool isHotNumber = false;

            foreach (NumberStatistics hotNumber in _FactorHotNumbers)
            {
                if (iNumberToCheck == hotNumber.Number)
                {
                    isHotNumber = true;
                    break;
                }
            }

            return isHotNumber;
        }

        private void CheckHitCount(string iFilename)
        {
            int[] winnings = new int[11];
            int[] totalWinnings = new int[11];
            double winningRafflesCounter = 0;
            double totalRaffles = 0;
            LotteryTable winningTable = null;
            //Compare between generated numbers and Real results

            Console.WriteLine(string.Format("Total tables generated: {0}\n", _WantedTables));
            Console.WriteLine("-----------------------------------------\n");

            //foreach (LotteryWinningResult winningResult in _LotteryHistoricResults)
            //{
            int totalResultsToScan = 308;
            for (int k = 0; k < totalResultsToScan/*_LotteryHistoricResults.Count*/; k++)
            {
                int i = 0;
                int strong = 0;

                bool isStrongNumberIdentical = false;

                while (i < _Batches.Count)
                {
                    for (int j = 0; j < _Batches[i].Count; j++)
                    {
                        int hitCount = CompareTables(_Batches[i].ElementAt(j), _LotteryHistoricResults.ElementAt(k), ref isStrongNumberIdentical);

                        if (isStrongNumberIdentical)
                        {
                            strong = 1;
                        }
                        else
                        {
                            strong = 0;
                        }

                        totalRaffles++;

                        if (hitCount > 2)
                        {
                            LotteryTable realTable = _LotteryHistoricResults.ElementAt(k);
                            LotteryTable generatedTable = _Batches[i].ElementAt(j);
                            Console.WriteLine(string.Format("Real winning table: {0},{1},{2},{3},{4},{5}, strong:{6}", realTable._Numbers[0],
                                                                                                                realTable._Numbers[1],
                                                                                                                realTable._Numbers[2],
                                                                                                                realTable._Numbers[3],
                                                                                                                realTable._Numbers[4],
                                                                                                                realTable._Numbers[5],
                                                                                                                realTable._StrongNumber));

                            Console.WriteLine(string.Format("Generated table: {0},{1},{2},{3},{4},{5}, strong:{6}", generatedTable._Numbers[0],
                                                                                                                generatedTable._Numbers[1],
                                                                                                                generatedTable._Numbers[2],
                                                                                                                generatedTable._Numbers[3],
                                                                                                                generatedTable._Numbers[4],
                                                                                                                generatedTable._Numbers[5],
                                                                                                                generatedTable._StrongNumber));
                            Console.WriteLine(string.Format("Table {0} from batch {1} yielded: {2}/6, strong: {3}/1", j, i, hitCount, strong));

                            if (isStrongNumberIdentical)
                            {
                                //0 1 2 3 4  5 6  7 8  9 10
                                //- - - 3 3+ 4 4+ 5 5+ 6 6+
                                if (hitCount == 3)
                                {
                                    winnings[hitCount + 1]++;
                                    totalWinnings[hitCount + 1]++;
                                }
                                else if (hitCount > 3)
                                {
                                    winnings[hitCount + 2]++;
                                    totalWinnings[hitCount + 2]++;
                                }
                            }
                            else
                            {
                                if (hitCount == 3)
                                {
                                    winnings[hitCount]++;
                                    totalWinnings[hitCount]++;
                                }
                                else if (hitCount > 3)
                                {
                                    winnings[hitCount + 1]++;
                                    totalWinnings[hitCount + 1]++;
                                }
                            }

                            winningRafflesCounter++;
                            winningTable = generatedTable;
                        }
                    }

                    i++;
                }

                //int sumWinnings = winnings[3] + winnings[4] + winnings[5] + winnings[6] + winnings[7] + winnings[8] + winnings[9] + winnings[10];
                //if (sumWinnings > 5)
                //{
                //    if (winningTable != null)
                //    {
                //        using (StreamWriter writer = new StreamWriter("winningNumbers", true))
                //        {

                //            writer.WriteLine(currTable);
                //        } 
                //    }
                //}

                Console.WriteLine(string.Format("3  3+  4  4+  5  5+  6  6+"));
                Console.WriteLine(string.Format("{0}  {1}  {2}  {3}  {4}  {5}  {6}  {7}", winnings[3], winnings[4], winnings[5], winnings[6], winnings[7], winnings[8], winnings[9], winnings[10]));
                Console.WriteLine("-------------------------------------------------------------------------\n");
                winnings = new int[11];
                winningTable = null;
            }

            Console.WriteLine("Total winnings out of {0} winning results", totalResultsToScan);
            Console.WriteLine("Sending {0} tables each time", _WantedTables);

            int prize = CalculateWinnings(totalWinnings);
            double moneySpent = _WantedTables * totalResultsToScan * 2.9;
            Console.WriteLine("Total money spent: {0}", moneySpent);
            Console.WriteLine("Total money won: {0}", prize);
            Console.WriteLine("Money Won\\Spent ratio {0}", prize / moneySpent);
            Console.WriteLine("Raffles Won: {0}/{1}", winningRafflesCounter, totalRaffles);
            Console.WriteLine("Raffles Won percentage: {0}", winningRafflesCounter / totalRaffles);

            Console.WriteLine(string.Format("3   3+   4   4+   5   5+   6   6+"));
            Console.WriteLine(string.Format("{0}   {1}   {2}   {3}   {4}   {5}   {6}   {7}", totalWinnings[3], totalWinnings[4], totalWinnings[5], totalWinnings[6], totalWinnings[7], totalWinnings[8], totalWinnings[9], totalWinnings[10]));

            using (StreamWriter writer = new StreamWriter(iFilename, true))
            {
                //writer.WriteLine("3, 3+, 4, 4+, 5, 5+, 6, 6+");

                string currTable = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                                     totalWinnings[3],
                                                     totalWinnings[4],
                                                     totalWinnings[5],
                                                     totalWinnings[6],
                                                     totalWinnings[7],
                                                     totalWinnings[8],
                                                     totalWinnings[9],
                                                     totalWinnings[10]);

                writer.WriteLine(currTable);
            }

            //winningTables.Clear();
        }

        private int CalculateWinnings(int[] iWinnings)
        {
            //3  3+ 4  4+  5   5+   6      6+
            //10 41 51 156 614 8382 500000
            int[] prizes = { 0, 0, 0, 10, 41, 51, 156, 614, 8382, 500000, 0 };

            int sum = 0;
            int i = 0;

            for (i = 3; i < iWinnings.Length; i++)
            {
                sum += iWinnings[i] * prizes[i];
            }

            return sum;
        }

        private int CompareTables(LotteryTable iLotteryTable1, LotteryTable iLotteryTable2, ref bool iIsStrongNumberIdentical)
        {
            int i = 0;
            int hitCount = 0;

            for (i = 0; i < iLotteryTable1._Numbers.Length; i++)
            {
                if (iLotteryTable2._Numbers.Contains(iLotteryTable1._Numbers[i]))
                {
                    hitCount++;
                }
            }

            if (iLotteryTable1._StrongNumber == iLotteryTable2._StrongNumber)
            {
                iIsStrongNumberIdentical = true;
            }
            else
            {
                iIsStrongNumberIdentical = false;
            }

            return hitCount;
        }

        private int Compare6NumbersTables(int[] iLotteryTable1, int[] iLotteryTable2, ref List<int> iNumbersHit)
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

        private int Compare6NumbersTables(int[] iLotteryTable1, int[] iLotteryTable2)
        {
            int i = 0;
            int hitCount = 0;

            for (i = 0; i < iLotteryTable1.Length; i++)
            {
                if (iLotteryTable2.Contains(iLotteryTable1[i]))
                {
                    hitCount++;
                }
            }

            return hitCount;
        }

        /// <summary>
        /// Reads all combinations from file and removes sequences (3 to 6 numbers sequence)
        /// </summary>
        /// <param name="iFilepath"></param>
        /// <param name="iRemoveSequencesAndSeries"></param>
        public List<int[]> ReadCombinationsFile(string iFilepath, bool iRemoveSequencesAndSeries)
        {
            string path = string.Empty;
            List<int[]> combinations = new List<int[]>();

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
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] lineSplit = line.Split(',');
                        int[] numbers = new int[6];

                        numbers[0] = int.Parse(lineSplit[0]);
                        numbers[1] = int.Parse(lineSplit[1]);
                        numbers[2] = int.Parse(lineSplit[2]);
                        numbers[3] = int.Parse(lineSplit[3]);
                        numbers[4] = int.Parse(lineSplit[4]);
                        numbers[5] = int.Parse(lineSplit[5]);

                        if (iRemoveSequencesAndSeries)
                        {
                            if (!IsSequence(numbers, 3) && !IsSeries(numbers, 3, 2, 15) && CountSingleDigitNumbers(numbers) < 4)
                            {
                                combinations.Add(numbers);
                            }
                        }
                        else
                        {
                            combinations.Add(numbers);
                        }
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("Path {0} doesn't exist", Path.GetDirectoryName(iFilepath)));
            }

            return combinations;
        }

        private int CountSingleDigitNumbers(int[] iArray)
        {
            int counter = 0;

            for (int i = 0; i < iArray.Length; i++)
            {
                if (iArray[i] < 10)
                {
                    counter++;
                }
            }

            return counter;
        }

        public void /*List<ShortLotteryTable>*/ RemoveAllSequences(int iFromSequence, int iToSequence)
        {
            List<ShortLotteryTable> sequences = new List<ShortLotteryTable>();


            for (int j = 0; j < _Combinations.Count; j++)
            {
                for (int i = iFromSequence; i < iToSequence; i++)
                {
                    //detect sequence
                    if (IsSequence(_Combinations[j]._Numbers, i))
                    {
                        bool isRemoved = _Combinations.Remove(_Combinations[j]);
                        ///sequences.Add(table);
                        break;
                    }
                }
            }

            //return sequences;
        }

        //public void RemoveAllSeries(int iMinSeries, int MinDiff)
        //{
        //    for (int j = 0; j < _Combinations.Count; j++)
        //    {
        //        for (int i = iFromSequence; i < iToSequence; i++)
        //        {
        //            //detect sequence
        //            if (IsSequence(_Combinations[j]._Numbers, i))
        //            {
        //                bool isRemoved = _Combinations.Remove(_Combinations[j]);
        //                ///sequences.Add(table);
        //                break;
        //            }
        //        }
        //    }

        //    //return sequences;
        //}

        private bool IsSequence(int[] iArray, int iWantedSequenceLength)
        {
            bool isSequence = false;
            int currSequenceLength = 1;
            List<int> sequences = new List<int>();

            int i = 0;
            while (i < iArray.Length - 1)
            {
                int res = iArray[i + 1] - iArray[i];

                if (res == 1)
                {
                    currSequenceLength++;
                }
                else
                {
                    if (currSequenceLength == iWantedSequenceLength)
                    {
                        isSequence = true;
                        break;
                    }
                }

                if (currSequenceLength == iWantedSequenceLength)
                {
                    isSequence = true;
                    break;
                }

                i++;
            }

            return isSequence;
        }

        private bool IsSeries(int[] iArray, int iMinSeries, int iMinDiff, int iMaxDiff)
        {
            bool isSeries = false;
            bool isStopLoop = false;

            List<int> sequences = new List<int>();

            int i = 0;
            for (int j = iMinDiff; j < iMaxDiff; j++)
            {
                int currSeriesLength = 1;
                while (i < iArray.Length - 1)
                {
                    int res = iArray[i + 1] - iArray[i];

                    if (res == j)
                    {
                        currSeriesLength++;
                    }
                    else
                    {
                        if (currSeriesLength == iMinSeries)
                        {
                            isSeries = true;
                            isStopLoop = true;
                            break;
                        }
                        else
                        {
                            currSeriesLength = 1;
                        }
                    }

                    if (currSeriesLength == iMinSeries)
                    {
                        isSeries = true;
                        isStopLoop = true;
                        break;
                    }

                    i++;
                }

                if (isStopLoop)
                {
                    break;
                }
                else
                {
                    i = 0;
                }
            }

            return isSeries;
        }

        public List<ShortLotteryTable> GetTablesWithSpecificNumbers(int[] iNumbers, bool iUsePreconfiguredNumbers)
        {
            List<ShortLotteryTable> tempList = new List<ShortLotteryTable>();
            int strongNumberCounter = 0;

            foreach (ShortLotteryTable table in _Combinations)
            {
                if (iUsePreconfiguredNumbers)
                {
                    foreach (NumberStatistics number in _FactorHotNumbers)
                    {
                        if (IsNumberExists(number.Number, table._Numbers))
                        {
                            strongNumberCounter++;
                        }
                    }

                    if (strongNumberCounter >= 2)
                    {
                        tempList.Add(table);
                    }

                    strongNumberCounter = 0;
                }
                else
                {
                    if (iNumbers != null && iNumbers.Length > 0)
                    {
                        foreach (int number in iNumbers)
                        {
                            if (IsNumberExists(number, table._Numbers))
                            {
                                strongNumberCounter++;
                            }
                        }
                    }

                    if (strongNumberCounter >= 2)
                    {
                        tempList.Add(table);
                    }

                    strongNumberCounter = 0;
                }
            }

            return tempList;
        }

        public int CountTablesWithHotNumbers(int iWantedNumbersInTable, int[] iNumbers, bool iUsePreconfiguredNumbers, int iNumRecordsToConsider)
        {
            int counter = 0;
            int strongNumberCounter = 0;

            for (int i = 0; i < iNumRecordsToConsider; i++)
            {
                if (iUsePreconfiguredNumbers)
                {
                    foreach (NumberStatistics number in _FactorHotNumbers)
                    {
                        if (IsNumberExists(number.Number, _LotteryHistoricResults[i]._Numbers))
                        {
                            strongNumberCounter++;
                        }
                    }

                    if (strongNumberCounter >= iWantedNumbersInTable)
                    {
                        counter++;
                    }

                    strongNumberCounter = 0;
                }
                else
                {
                    if (iNumbers != null && iNumbers.Length > 0)
                    {
                        foreach (int number in iNumbers)
                        {
                            if (IsNumberExists(number, _LotteryHistoricResults[i]._Numbers))
                            {
                                strongNumberCounter++;
                            }
                        }
                    }

                    if (strongNumberCounter >= iWantedNumbersInTable)
                    {
                        counter++;
                    }

                    strongNumberCounter = 0;
                }

            }

            return counter;
        }

        /// <summary>
        /// This function will give a dispersion across the different regions
        /// So, if the numbers are: 1 7 12 19 21 36
        /// The dispersion is: 2 2 1 1
        /// For 3 12 22 29 31 37 the dispersion is: 1 1 2 2 and so on...
        /// <10
        /// 1 2 3 4 5 6 7 8 9
        /// <20
        /// 10 11 12 13 14 15 16 17 18 19
        /// <30
        /// 20 21 22 23 24 25 26 27 28 29
        /// <38
        /// 30 31 32 33 34 35 36 37
        /// </summary>
        /// <param name="iNumberRecordsToConsider"></param>
        public void GetDispersionByTenthPattern(int iNumberRecordsToConsider)
        {
            List<ResultDispersion> dispersion = new List<ResultDispersion>();
            int lessThan10 = 0;
            int lessThan20 = 0;
            int lessThan30 = 0;
            int lessThan38 = 0;
            double totalPercentage = 0;
            int numberOfRecordsToConsider = iNumberRecordsToConsider;

            if (numberOfRecordsToConsider > _LotteryHistoricResults.Count)
            {
                numberOfRecordsToConsider = _LotteryHistoricResults.Count;
            }

            for (int i = 0; i < numberOfRecordsToConsider; i++)
            {
                foreach (int number in _LotteryHistoricResults[i]._Numbers)
                {
                    if (number < 10)
                    {
                        lessThan10++;
                    }
                    else if (number >= 10 && number < 20)
                    {
                        lessThan20++;
                    }
                    else if (number >= 20 && number < 30)
                    {
                        lessThan30++;
                    }
                    else if (number >= 30 && number < 38)
                    {
                        lessThan38++;
                    }
                }

                dispersion.Add(new ResultDispersion()
                {
                    LessThan10 = lessThan10,
                    LessThan20 = lessThan20,
                    LessThan30 = lessThan30,
                    LessThan38 = lessThan38,
                    Numbers = _LotteryHistoricResults[i]._Numbers
                });

                lessThan10 = 0;
                lessThan20 = 0;
                lessThan30 = 0;
                lessThan38 = 0;
            }

            //categorize all different dispersions into groups
            MultiMap<ResultDispersion> mp = new MultiMap<ResultDispersion>();
            foreach (ResultDispersion item in dispersion)
            {
                string pattern = item.LessThan10.ToString() + item.LessThan20.ToString() + item.LessThan30.ToString() + item.LessThan38.ToString();
                mp.Add(pattern, item);
            }

            foreach (string pattern in mp.Keys)
            {
                using (StreamWriter writer = new StreamWriter("dispersion.txt", true))
                {
                    double percentage = Math.Round((double)mp[pattern].Count / numberOfRecordsToConsider, 3);
                    totalPercentage += percentage;
                    string line = string.Format("{0},{1},{2}", pattern, mp[pattern].Count, percentage);
                    writer.WriteLine(line);
                }
            }

        }

        public void GetDispersionByTenthPattern(List<int[]> iCombinations, int iNumberRecordsToConsider, string iFilename)
        {
            List<ResultDispersion> dispersion = new List<ResultDispersion>();
            int lessThan10 = 0;
            int lessThan20 = 0;
            int lessThan30 = 0;
            int lessThan38 = 0;
            double totalPercentage = 0;
            int numberOfRecordsToConsider = iNumberRecordsToConsider;

            if (numberOfRecordsToConsider > iCombinations.Count)
            {
                numberOfRecordsToConsider = iCombinations.Count;
            }

            for (int i = 0; i < numberOfRecordsToConsider; i++)
            {
                foreach (int number in iCombinations[i])
                {
                    if (number < 10)
                    {
                        lessThan10++;
                    }
                    else if (number >= 10 && number < 20)
                    {
                        lessThan20++;
                    }
                    else if (number >= 20 && number < 30)
                    {
                        lessThan30++;
                    }
                    else if (number >= 30 && number < 38)
                    {
                        lessThan38++;
                    }
                }

                dispersion.Add(new ResultDispersion()
                {
                    LessThan10 = lessThan10,
                    LessThan20 = lessThan20,
                    LessThan30 = lessThan30,
                    LessThan38 = lessThan38,
                    Numbers = iCombinations[i]
                });

                lessThan10 = 0;
                lessThan20 = 0;
                lessThan30 = 0;
                lessThan38 = 0;
            }

            //categorize all different dispersions into groups
            MultiMap<ResultDispersion> mp = new MultiMap<ResultDispersion>();
            foreach (ResultDispersion item in dispersion)
            {
                string pattern = item.LessThan10.ToString() + item.LessThan20.ToString() + item.LessThan30.ToString() + item.LessThan38.ToString();
                mp.Add(pattern, item);
            }

            foreach (string pattern in mp.Keys)
            {
                using (StreamWriter writer = new StreamWriter(iFilename, true))
                {
                    double percentage = Math.Round((double)mp[pattern].Count / numberOfRecordsToConsider, 3);
                    totalPercentage += percentage;
                    string line = string.Format("{0},{1},{2}", pattern, mp[pattern].Count, percentage);
                    writer.WriteLine(line);
                }
            }

        }

        public void ReadDispersionFile(string iFilepath)
        {
            string path = string.Empty;
            _Dispersion = new List<Dispersion>();

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
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] lineSplit = line.Split(',');

                        if (lineSplit.Length == 3)
                        {
                            string s = lineSplit[0];
                            int lessThan10 = int.Parse(lineSplit[0].ElementAt(0).ToString());
                            int lessThan20 = int.Parse(lineSplit[0].ElementAt(1).ToString());
                            int lessThan30 = int.Parse(lineSplit[0].ElementAt(2).ToString());
                            int lessThan38 = int.Parse(lineSplit[0].ElementAt(3).ToString());
                            double percentage = double.Parse(lineSplit[2]);

                            _Dispersion.Add(new Dispersion()
                            {
                                Pattern = s,
                                Percentage = percentage,
                                LessThan10 = lessThan10,
                                LessThan20 = lessThan20,
                                LessThan30 = lessThan30,
                                LessThan38 = lessThan38,
                                Incidence = int.Parse(lineSplit[1])
                            });
                        }
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("Path {0} doesn't exist", Path.GetDirectoryName(iFilepath)));
            }
        }

        public List<ShortLotteryTable> GetCombinationsWithDispersionPercentage(List<ShortLotteryTable> iCombinations, double iMinimumWantedPercentage)
        {
            List<ShortLotteryTable> matches = null;

            if (iCombinations != null && iCombinations.Count > 0)
            {
                if (_Dispersion != null && _Dispersion.Count > 0)
                {
                    matches = new List<ShortLotteryTable>();
                    //first, go over the dispersion list to see which patterns adhere to the percentage
                    List<Dispersion> neededPatterns = new List<Dispersion>();
                    foreach (Dispersion dispersion in _Dispersion)
                    {
                        if (dispersion.Percentage >= iMinimumWantedPercentage)
                        {
                            neededPatterns.Add(dispersion);
                        }
                    }

                    //Then, go over the combinations to see which ones match the patterns
                    foreach (ShortLotteryTable table in iCombinations)
                    {

                        foreach (Dispersion pattern in neededPatterns)
                        {
                            if (pattern.Pattern == GetTablePattern(table))
                            {
                                matches.Add(table);
                            }
                        }
                    }
                }
            }

            return matches;
        }

        private string GetTablePattern(ShortLotteryTable iCombination)
        {
            string pattern;
            int lessThan10 = 0;
            int lessThan20 = 0;
            int lessThan30 = 0;
            int lessThan38 = 0;

            foreach (int number in iCombination._Numbers)
            {
                if (number < 10)
                {
                    lessThan10++;
                }
                else if (number >= 10 && number < 20)
                {
                    lessThan20++;
                }
                else if (number >= 20 && number < 30)
                {
                    lessThan30++;
                }
                else if (number >= 30 && number < 38)
                {
                    lessThan38++;
                }
            }

            pattern = lessThan10.ToString() + lessThan20.ToString() + lessThan30.ToString() + lessThan38.ToString();

            return pattern;
        }

        public void CheckCombinationsAgainstResults(List<ShortLotteryTable> iCombinations, int iMinWinningStreak, int iMaxWinningStreak)
        {
            for (int i = 0; i < iCombinations.Count; i++)
            {
                int[] winnings = new int[7];
                for (int j = 0; j < _LotteryHistoricResults.Count; j++)
                {
                    List<int> numbersHit = new List<int>();
                    int result = Compare6NumbersTables(iCombinations[i]._Numbers, _LotteryHistoricResults[j]._Numbers, ref numbersHit);
                    if (result >= iMinWinningStreak)
                    {
                        switch (result)
                        {
                            case 3:
                                winnings[3]++;
                                break;
                            case 4:
                                winnings[4]++;
                                break;
                            case 5:
                                winnings[5]++;
                                break;
                            case 6:
                                winnings[6]++;
                                break;
                        }
                    }
                }

                //write results to file
                if (winnings[3] != 0 || winnings[4] != 0 || winnings[5] != 0 || winnings[6] != 0)
                {
                    using (StreamWriter writer = new StreamWriter("WinningsSummary.txt", true))
                    {
                        string line = string.Format("{0};{1};{2};{3};{4};{5},{6},{7},{8},{9}", iCombinations[i]._Numbers[0],
                                                                                                   iCombinations[i]._Numbers[1],
                                                                                                   iCombinations[i]._Numbers[2],
                                                                                                   iCombinations[i]._Numbers[3],
                                                                                                   iCombinations[i]._Numbers[4],
                                                                                                   iCombinations[i]._Numbers[5],
                                                                                                   winnings[3],
                                                                                                   winnings[4],
                                                                                                   winnings[5],
                                                                                                   winnings[6]);
                        writer.WriteLine(line);
                    }
                }
            }
        }

        public List<PartialLotteryTable> ReadPartialCombinationsFile(string iFilename, bool iReverse)
        {
            string path = string.Empty;
            List<PartialLotteryTable> combinations = new List<PartialLotteryTable>();

            if (Path.GetDirectoryName(iFilename) == string.Empty)
            {
                path = Path.Combine(System.Environment.CurrentDirectory, iFilename);
            }
            else
            {
                path = iFilename;
            }

            if (Directory.Exists(Path.GetDirectoryName(path)))
            {
                using (StreamReader reader = new StreamReader(path, true))
                {
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] lineSplit = line.Split(',');
                        PartialLotteryTable currTable = new PartialLotteryTable();

                        for (int j = 0; j < lineSplit.Length - 1; j++)
                        {
                            currTable.Numbers[j] = int.Parse(lineSplit[j]);
                        }

                        currTable.Commonness = int.Parse(lineSplit[lineSplit.Length - 1]);

                        combinations.Add(currTable);
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("Path {0} doesn't exist", Path.GetDirectoryName(iFilename)));
            }

            List<PartialLotteryTable> sortedList = combinations.OrderBy(plt => plt.Commonness).ToList<PartialLotteryTable>();

            if (iReverse)
            {
                sortedList.Reverse();
            }

            return sortedList;
        }

        public List<int[]> GenerateRemainingNumbers(int[] iNumbers)
        {
            List<int[]> selectedNumbers = new List<int[]>();
            List<int> usedNumbers = new List<int>();

            //Pick the number that appears with the numbers in iNumbers most times
            //for each number in the 3tuple, create a list with the commonest number
            //Then go over the lists (first 3, then 4, finally 5) and choose the number
            //that is the commonest for all 3, 4, 5 numbers in accordance

            List<NumberCommoness> first = GetSortedCommonessForSpecificNumber(iNumbers[0], true);
            List<NumberCommoness> second = GetSortedCommonessForSpecificNumber(iNumbers[1], true);
            List<NumberCommoness> third = GetSortedCommonessForSpecificNumber(iNumbers[2], true);

            List<NumberCommoness> fourthNumber = PickNumberInPlace4(iNumbers, first, second, third);
            List<NumberCommoness> fifthNumber;
            List<NumberCommoness> sixthNumber;
            foreach (NumberCommoness number4 in fourthNumber)
            {
                usedNumbers.Add(number4.Number);
                fifthNumber = PickNumberInPlace5(iNumbers, first, second, third,
                                                 GetSortedCommonessForSpecificNumber(number4.Number, true),
                                                 usedNumbers);

                foreach (NumberCommoness number5 in fifthNumber)
                {
                    usedNumbers.Add(number5.Number);
                    sixthNumber = PickNumberInPlace6(iNumbers, first, second, third,
                                                                   GetSortedCommonessForSpecificNumber(number4.Number, true),
                                                                   GetSortedCommonessForSpecificNumber(number5.Number, true),
                                                                   usedNumbers);

                    foreach (NumberCommoness number6 in sixthNumber)
                    {
                        selectedNumbers.Add(new int[] { iNumbers[0], iNumbers[1], iNumbers[2], number4.Number, number5.Number, number6.Number });
                    }
                }
            }

            for (int i = 0; i < selectedNumbers.Count; i++)
            {
                selectedNumbers[i] = (from element in selectedNumbers[i] orderby element ascending select element)
               .ToArray();
            }

            return selectedNumbers;
        }

        private List<NumberCommoness> PickNumberInPlace6(int[] iNumbers,
                                       List<NumberCommoness> iFirstNumberCommoness,
                                       List<NumberCommoness> iSecondNumberCommoness,
                                       List<NumberCommoness> iThirdNumberCommoness,
                                       List<NumberCommoness> iFourthNumberCommoness,
                                       List<NumberCommoness> iFifthNumberCommoness,
                                       List<int> iUsedNumbers)
        {
            //Pick the number that appears the most times with all 3 numbers
            List<NumberCommoness> nextNumberOptions = new List<NumberCommoness>();

            //***************************************************************************************
            //Picking number in place 4
            for (int i = 1; i < 37; i++)
            {
                NumberCommoness currNumber = iFirstNumberCommoness[i];

                NumberCommoness secondNumber = iSecondNumberCommoness.Single(x => x.Number == currNumber.Number);
                NumberCommoness thirdNumber = iThirdNumberCommoness.Single(x => x.Number == currNumber.Number);
                NumberCommoness fourthNumber = iFourthNumberCommoness.Single(x => x.Number == currNumber.Number);
                NumberCommoness fifthNumber = iFifthNumberCommoness.Single(x => x.Number == currNumber.Number);

                if (!iNumbers.Contains(currNumber.Number) && !IsContainsNumber(currNumber.Number, iUsedNumbers))
                {
                    List<NumberCommoness> currSelection = new List<NumberCommoness>();
                    currSelection.Add(currNumber);
                    currSelection.Add(secondNumber);
                    currSelection.Add(thirdNumber);
                    currSelection.Add(fourthNumber);
                    currSelection.Add(fifthNumber);

                    currSelection = currSelection.OrderBy(x => x.Commoness).ToList();
                    nextNumberOptions.Add(currSelection[0]);
                }
            }

            //List of numbers created - now choose from list
            nextNumberOptions = nextNumberOptions.OrderBy(x => x.Commoness).Reverse().ToList();
            List<NumberCommoness> pickedNumbers = nextNumberOptions.Where(x => x.Commoness == nextNumberOptions[0].Commoness).ToList();

            return pickedNumbers;
        }

        private List<NumberCommoness> PickNumberInPlace5(int[] iNumbers,
                                       List<NumberCommoness> iFirstNumberCommoness,
                                       List<NumberCommoness> iSecondNumberCommoness,
                                       List<NumberCommoness> iThirdNumberCommoness,
                                       List<NumberCommoness> iFourthNumberCommoness,
                                       List<int> iUsedNumbers)
        {
            //Pick the number that appears the most times with all 3 numbers
            List<NumberCommoness> nextNumberOptions = new List<NumberCommoness>();

            //***************************************************************************************
            //Picking number in place 4
            for (int i = 1; i < 37; i++)
            {
                NumberCommoness currNumber = iFirstNumberCommoness[i];

                NumberCommoness secondNumber = iSecondNumberCommoness.Single(x => x.Number == currNumber.Number);
                NumberCommoness thirdNumber = iThirdNumberCommoness.Single(x => x.Number == currNumber.Number);
                NumberCommoness fourthNumber = iFourthNumberCommoness.Single(x => x.Number == currNumber.Number);

                if (!iNumbers.Contains(currNumber.Number) && !IsContainsNumber(currNumber.Number, iUsedNumbers))
                {
                    List<NumberCommoness> currSelection = new List<NumberCommoness>();
                    currSelection.Add(currNumber);
                    currSelection.Add(secondNumber);
                    currSelection.Add(thirdNumber);
                    currSelection.Add(fourthNumber);

                    currSelection = currSelection.OrderBy(x => x.Commoness).ToList();
                    nextNumberOptions.Add(currSelection[0]);
                }
            }

            //List of numbers created - now choose from list
            nextNumberOptions = nextNumberOptions.OrderBy(x => x.Commoness).Reverse().ToList();
            List<NumberCommoness> pickedNumbers = nextNumberOptions.Where(x => x.Commoness == nextNumberOptions[0].Commoness).ToList();

            return pickedNumbers;
        }

        private List<NumberCommoness> PickNumberInPlace4(int[] iNumbers,
                                       List<NumberCommoness> iFirstNumberCommoness,
                                       List<NumberCommoness> iSecondNumberCommoness,
                                       List<NumberCommoness> iThirdNumberCommoness)
        {
            //Pick the number that appears the most times with all 3 numbers
            List<NumberCommoness> nextNumberOptions = new List<NumberCommoness>();


            //***************************************************************************************
            //Picking number in place 4
            for (int i = 1; i < 37; i++)
            {
                NumberCommoness currNumber = iFirstNumberCommoness[i];

                NumberCommoness secondNumber = iSecondNumberCommoness.Single(x => x.Number == currNumber.Number);
                NumberCommoness thirdNumber = iThirdNumberCommoness.Single(x => x.Number == currNumber.Number);


                if (!iNumbers.Contains(currNumber.Number))
                {
                    List<NumberCommoness> currSelection = new List<NumberCommoness>();
                    currSelection.Add(currNumber);
                    currSelection.Add(secondNumber);
                    currSelection.Add(thirdNumber);

                    currSelection = currSelection.OrderBy(x => x.Commoness).ToList();
                    nextNumberOptions.Add(currSelection[0]);
                }
            }

            //List of numbers created - now choose from list
            nextNumberOptions = nextNumberOptions.OrderBy(x => x.Commoness).Reverse().ToList();
            List<NumberCommoness> pickedNumbers = nextNumberOptions.Where(x => x.Commoness == nextNumberOptions[0].Commoness).ToList();

            return pickedNumbers;
        }
        #endregion
    }
}
