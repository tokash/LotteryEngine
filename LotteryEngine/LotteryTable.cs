using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryEngine
{
    public class LotteryTable
    {
        public int[] _Numbers = new int[6];
        public int _StrongNumber = 0;
    }

    public class LotteryWinningResult: LotteryTable
    {
        public DateTime _LotteryDate { get; set; }
        public string _LotteryRaffleID { get; set; }
        public int _HitCount { get; set; }
    }

    public class ShortLotteryTable
    {
        public int[] _Numbers = new int[6];
    }

    public class PartialLotteryTable
    {
        public int[] Numbers = new int[6];
        public int Commonness;
    }

    public class NumberCommoness
    {
        public int Number;
        public int Commoness;
        public int Rank = 1;
    }

    public class ChosenLotteryTable
    {
        public int[] Numbers = new int[6];
        public int[] Ranks = new int[5];
        public int Leading;
        public List<int> NumbersHit = new List<int>();
        public int HitCount;
        public int StrongNumber;
    }

    class DistinctItemComparer : IEqualityComparer<int[]>
    {

        public bool Equals(int[] x, int[] y)
        {
            bool isEqual = true;

            //Console.WriteLine(string.Format("{0} - {1}", x.Length, y.Length));
            if (x.Length == y.Length)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i])
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            else
            {
                isEqual = false;
            }

            return isEqual;
        }

        public int GetHashCode(int[] obj)
        {
            return base.GetHashCode();
            //return obj.Id.GetHashCode() ^
            //    obj.Name.GetHashCode() ^
            //    obj.Code.GetHashCode() ^
            //    obj.Price.GetHashCode();
        }
    }

    class DistinctChosenLotteryTableComparer : IEqualityComparer<ChosenLotteryTable>
    {

        public bool Equals(ChosenLotteryTable x, ChosenLotteryTable y)
        {
            bool isEqual = true;

            //Console.WriteLine(string.Format("{0} - {1}", x.Length, y.Length));
            if (x.Numbers.Length == y.Numbers.Length)
            {
                for (int i = 0; i < x.Numbers.Length; i++)
                {
                    if (x.Numbers[i] != y.Numbers[i])
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            else
            {
                isEqual = false;
            }

            return isEqual;
        }

        public int GetHashCode(ChosenLotteryTable obj)
        {
            return base.GetHashCode();
            //return obj.Id.GetHashCode() ^
            //    obj.Name.GetHashCode() ^
            //    obj.Code.GetHashCode() ^
            //    obj.Price.GetHashCode();
        }
    }
}
