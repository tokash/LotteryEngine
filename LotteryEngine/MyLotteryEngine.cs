using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryEngine
{
    class MyLotteryEngine
    {
        List<LotteryTable> _LotteryTables = new List<LotteryTable>();
        List<List<LotteryTable>> _Batches = new List<List<LotteryTable>>();
        List<NumberStatistics> _HotNumbers = new List<NumberStatistics>();
        List<LotteryWinningResult> _LotteryHistoricResults = new List<LotteryWinningResult>();
        Random _RandomNumbersGenerator = new Random();
        List<ShortLotteryTable> _Combinations = new List<ShortLotteryTable>();

        public List<ShortLotteryTable> Combinations
        {
            get
            {
                return _Combinations;
            }
        }

        //Generate 5 batches of 6 lottery numbers
        //Replace all numbers which are not "strong", with strong number according to their factor

        //Compare results with "real" results from csv file

        //Most popular numbers since the latest change, 6 numbers and one strong number (14/5/2011) until

        //int[] Numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 ,26 , 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37 };
                                               //1   2   3   4   5   6   7   8   9   10  11  12  13  14  15  16  17  18  19  20  21  22  23  24  25  26  27  28  29  30  31  32  33  34  35  36  37
        int[] NumbersDispersion = new int[] { 0, 53, 56, 57, 44, 41, 35, 52, 53, 41, 46, 48, 55, 40, 37, 48, 53, 48, 41, 51, 47, 60, 38, 47, 51, 54, 63, 48, 46, 48, 53, 44, 43, 52, 49, 54, 57, 53 };

        int[] HotNumbers = new int[] {2, 3, 12, 21, 25, 26, 35, 36 };
        int[] HotNumbersDispersion = new int[] {55, 57, 57, 62, 54, 63, 55, 57};
        int _NumberOfRuffles = 308;

        int[] _StringHotNumbers = new int[] {1, 3, 5};
        int[] _Repetitions = null;

        int _WantedTables = 0;

        public MyLotteryEngine(string iLotteryResultsFilepath, int iWantedTables)
        {
            //int i = 0;
            //foreach (int number in HotNumbers)
            //{
            //    _HotNumbers.Add(new NumberStatistics() { Number = number, Factor = (int)Math.Floor((double)_NumberOfRuffles / HotNumbersDispersion[i]) });
            //    i++;
            //}
            _WantedTables = iWantedTables;

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
                    _HotNumbers.Add(new NumberStatistics() { Number = i, Factor = factor });
                }
                else
                {
                    factor = (int)Math.Round((NumbersDispersion[i] / (double)_NumberOfRuffles / (_WantedTables - 6)) * 1000);
                }
            }
            
            ReadLotteryOfficialResultFile(iLotteryResultsFilepath);
            //_LotteryHistoricResults.Reverse();
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
                    
                    _Batches.Add( new List<LotteryTable>(_LotteryTables));
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

                                    if (hotNumber.Factor == currBatchRepetitions[hotNumber.Number] )
	                                {
		                                isReachedMaxRepetitions = true;
	                                }

                                    if (selectedHotNumbers.Count == _HotNumbers.Count)
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
            int placeInList = _RandomNumbersGenerator.Next(0, _HotNumbers.Count);

            return _HotNumbers.ElementAt(placeInList);
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
            int[] numbers = new int[6] {0,0,0,0,0,0};
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

            foreach (NumberStatistics hotNumber in _HotNumbers)
            {
                if (iNumberToCheck == hotNumber.Number)
                {
                    isHotNumber = true;
                    break;
                }
            }

            return isHotNumber;
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
                        }

                        //0 - ID, 1 - Date, 2-7 numbers, 8 string number
                        _LotteryHistoricResults.Add(new LotteryWinningResult() {
                                                     _LotteryRaffleID = lineSplit[0],
                                                     _LotteryDate = currDate,
                                                     _Numbers = numbers,
                                                     _StrongNumber = int.Parse(lineSplit[8])});
                        
                        i++;
                    }                    
                }
            }
            else
            {
                throw new Exception(string.Format("Path {0} doesn't exist", Path.GetDirectoryName(iFilepath)));
            }
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
                    for (int j = 0; j < _Batches[i].Count; j++ )
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
                                else if(hitCount > 3)
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
            Console.WriteLine("Money Won\\Spent ratio {0}", prize/moneySpent);
            Console.WriteLine("Raffles Won: {0}/{1}", winningRafflesCounter, totalRaffles);
            Console.WriteLine("Raffles Won percentage: {0}", winningRafflesCounter/totalRaffles);

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
            int[] prizes = {0,0,0,10,41, 51,156,614, 8382, 500000, 0};

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

        /// <summary>
        /// Reads all combinations from file and removes sequences (3 to 6 numbers sequence)
        /// </summary>
        /// <param name="iFilepath"></param>
        /// <param name="iRemoveSequencesAndSeries"></param>
        public void ReadCombinationsFile(string iFilepath, bool iRemoveSequencesAndSeries)
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
                                _Combinations.Add(new ShortLotteryTable()
                                {
                                    _Numbers = numbers,
                                });
                            }
                        }
                        else
                        {
                            _Combinations.Add(new ShortLotteryTable()
                            {
                                _Numbers = numbers,
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


            for (int j = 0; j < _Combinations.Count;  j++ )
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
    }
}
