using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

//horizontal gap between cells = 19
//vertical gap between cells = 9
//gap between tables = 

//table1
//1 - 7
//1 - (71, 63, 79, 63)
//2 - (90, 63, 98, 63)
//3 - (109, 63, 117, 63)

//8 - 17
//8 - (16, 72, 24, 72)
//9 - (35, 72, 43, 72)

//18 - 27

//28 - 37

//table 2 (y - (original + i * 4 * 9) + 1)
//1 - (71, 107, 79, 107)

//table 3
//1 - (71, 139, 79, 139)

//table 4
//1 - (71, 176, 79, 176)

//table 5
//1 - (71, 214, 79, 214)

namespace LotteryTablePrinter
{
    class LotteryPrinter
    {
        #region Members
        Application _ApplicationWord = new Microsoft.Office.Interop.Word.Application();
        Document _WordDocument = null;
        List<LotteryTablePosition> _Tables = new List<LotteryTablePosition>();
        int _CellHorizontalGap = 19;
        int _CellVerticalGap = 9;
        int _MarkLength = 8; 
        #endregion

        public LotteryPrinter(string iDocumentPath)
        {
            if (File.Exists(iDocumentPath))
            {
                _WordDocument = _ApplicationWord.Documents.Open(iDocumentPath);

                InitLotteryTablePosition();
            }
        }

        private void InitLotteryTablePosition()
        {
            //Adding table 1
            LotteryTableRow tableRow11 = CreateTableRow(1, 71, 63);
            LotteryTableRow tableRow12 = CreateTableRow(8, 16, 72);
            LotteryTableRow tableRow13 = CreateTableRow(18, 16, 81);
            LotteryTableRow tableRow14 = CreateTableRow(28, 16, 90);

            List<LotteryTableRow> table1Rows = new List<LotteryTableRow>();
            table1Rows.Add(tableRow11);
            table1Rows.Add(tableRow12);
            table1Rows.Add(tableRow13);
            table1Rows.Add(tableRow14);

            LotteryTablePosition table1Pos = CreateTablePosition(1, table1Rows);
            _Tables.Add(table1Pos);

            //Adding table 2
            LotteryTableRow tableRow21 = CreateTableRow(1, 71, 107);
            LotteryTableRow tableRow22 = CreateTableRow(8, 16, 116);
            LotteryTableRow tableRow23 = CreateTableRow(18, 16, 125);
            LotteryTableRow tableRow24 = CreateTableRow(28, 16, 134);

            List<LotteryTableRow>  table2Rows = new List<LotteryTableRow>();
            table2Rows.Add(tableRow21);
            table2Rows.Add(tableRow22);
            table2Rows.Add(tableRow23);
            table2Rows.Add(tableRow24);

            LotteryTablePosition table2Pos = CreateTablePosition(2, table2Rows);
            _Tables.Add(table2Pos);
        }

        private LotteryTablePosition CreateTablePosition(int iTableNumber, List<LotteryTableRow> iLotteryTableRows)
        {
            //adding tables to list
            LotteryTablePosition tablePos = new LotteryTablePosition();

            tablePos.TableNumber = iTableNumber;
            tablePos.TableRows = iLotteryTableRows;

            return tablePos;
        }

        private LotteryTableRow CreateTableRow(int iRowNumber, int x, int y)
        {
            LotteryTableRow tableRow = new LotteryTableRow();

            tableRow.RowNumber = iRowNumber;
            tableRow.StartingX = x;
            tableRow.StartingY = y;

            return tableRow;
        }

        public bool InsertShapeAtSpecificLocation(int iX, int iY, int iGap)
        {
            Microsoft.Office.Interop.Word.Range range = _WordDocument.Application.Selection.Range;

            Shape lineShape = range.Document.Shapes.AddLine(iX, iY, iX + iGap, iY);
            //lineShape.Width = 15;

            bool isSucceeded = false;
            if (lineShape != null)
            {
                isSucceeded = true;
            }

            return isSucceeded;
        }

        public void MarkTableAt(int iTableNumber, int[] iTable)
        {
            int x, y;

            for (int i = 0; i < iTable.Length; i++)
            {
                //need to put a mark at the place of the number in the form
                //Locate table in tables list, then calculate the mark place according to table rows
                LotteryTableRow row = GetMatchingTableRowPosition(iTableNumber, iTable[i]);

                if (iTable[i] != 1 || iTable[i] != 8 || iTable[i] != 18 || iTable[i] != 28)
                {
                    x = row.StartingX + _CellHorizontalGap * (iTable[i] - row.RowNumber) - ((iTable[i] - row.RowNumber)/2);
                    y = row.StartingY + 1;
                }
                else
                {
                    x = row.StartingX;
                    y = row.StartingY;
                }

                InsertShapeAtSpecificLocation(x, y, _MarkLength);
            }
        }

        private LotteryTableRow GetMatchingTableRowPosition(int iTableNumber, int iNumber)
        {
            LotteryTableRow row;

            if (iNumber < 8)
            {
                row = _Tables.Single(x => x.TableNumber == iTableNumber).TableRows.Single(x => x.RowNumber == 1);
            }
            else if(iNumber < 18)
            {
                row = _Tables.Single(x => x.TableNumber == iTableNumber).TableRows.Single(x => x.RowNumber == 8);
            }
            else if(iNumber < 28)
            {
                row = _Tables.Single(x => x.TableNumber == iTableNumber).TableRows.Single(x => x.RowNumber == 18);
            }
            else
            {
                row = _Tables.Single(x => x.TableNumber == iTableNumber).TableRows.Single(x => x.RowNumber == 28);
            }

            return row;
        }

        public void CloseDocument()
        {
            _WordDocument.Close();
        }
    }
}
