using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryTablePrinter
{
    public class LotteryTablePosition
    {
        public int TableNumber { get; set; }
        public List<LotteryTableRow> TableRows { get; set; }
    }

    public class LotteryTableRow
    {
        public int RowNumber { get; set; }
        public int StartingX { get; set; }
        public int StartingY { get; set; }
    }
}
