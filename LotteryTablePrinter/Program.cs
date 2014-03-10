using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryTablePrinter
{
    class Program
    {
        static void Main(string[] args)
        {
            LotteryPrinter lp = new LotteryPrinter(@"D:\gitRepository\LotteryEngine\LotteryTablePrinter\bin\Debug\Loto - Copy.docx");

            lp.MarkTableAt(1, new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37});
            lp.CloseDocument();
        }
    }
}
