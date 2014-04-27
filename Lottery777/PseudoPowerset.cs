using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotteryEngine;

namespace Lottery777
{
    class PseudoPowerset
    {
        private List<int[]> combinations = new List<int[]>();
        public List<int[]> Combinations
        {
            get { return combinations; }
        }

        public void GenerateSubsets(int[] set, int k, ref List<int[]> oCombinations)
        {
            int[] subset = new int[k];
            ProcessLargerSubsets(set, subset, 0, 0, ref oCombinations);
        }

        void ProcessLargerSubsets(int[] set, int[] subset, int subsetSize, int nextIndex, ref List<int[]> oCombinations)
        {
            if (subsetSize == subset.Length)
            {
                oCombinations.Add((int[])subset.Clone());
            }
            else
            {

                for (int j = nextIndex; j < set.Length; j++)
                {
                    subset[subsetSize] = set[j];
                    ProcessLargerSubsets(set, subset, subsetSize + 1, j + 1, ref oCombinations);
                }
            }
        }

    }
}
