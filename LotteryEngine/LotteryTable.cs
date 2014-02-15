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
    }

    public class ShortLotteryTable
    {
        public int[] _Numbers = new int[6];
    }
}
