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
        int _CellHorizontalGap = 18;
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
            LotteryTableRow tableRow11 = CreateTableRow(1, 68, 70);
            LotteryTableRow tableRow12 = CreateTableRow(8, 13, 81);
            LotteryTableRow tableRow13 = CreateTableRow(18, 13, 91);
            LotteryTableRow tableRow14 = CreateTableRow(28, 13, 102);

            List<LotteryTableRow> table1Rows = new List<LotteryTableRow>();
            table1Rows.Add(tableRow11);
            table1Rows.Add(tableRow12);
            table1Rows.Add(tableRow13);
            table1Rows.Add(tableRow14);

            LotteryTablePosition table1Pos = CreateTablePosition(1, table1Rows);
            _Tables.Add(table1Pos);

            ////Adding table 2
            LotteryTableRow tableRow21 = CreateTableRow(1, 67, 112);
            LotteryTableRow tableRow22 = CreateTableRow(8, 12, 123);
            LotteryTableRow tableRow23 = CreateTableRow(18, 12, 133);
            LotteryTableRow tableRow24 = CreateTableRow(28, 12, 143);

            List<LotteryTableRow> table2Rows = new List<LotteryTableRow>();
            table2Rows.Add(tableRow21);
            table2Rows.Add(tableRow22);
            table2Rows.Add(tableRow23);
            table2Rows.Add(tableRow24);

            LotteryTablePosition table2Pos = CreateTablePosition(2, table2Rows);
            _Tables.Add(table2Pos);

            //Adding table 3
            LotteryTableRow tableRow31 = CreateTableRow(1, 67, 155);
            LotteryTableRow tableRow32 = CreateTableRow(8, 12, 166);
            LotteryTableRow tableRow33 = CreateTableRow(18, 12, 176);
            LotteryTableRow tableRow34 = CreateTableRow(28, 12, 187);

            List<LotteryTableRow> table3Rows = new List<LotteryTableRow>();
            table3Rows.Add(tableRow31);
            table3Rows.Add(tableRow32);
            table3Rows.Add(tableRow33);
            table3Rows.Add(tableRow34);

            LotteryTablePosition table3Pos = CreateTablePosition(3, table3Rows);
            _Tables.Add(table3Pos);

            ////Adding table 4
            LotteryTableRow tableRow41 = CreateTableRow(1, 67, 196);
            LotteryTableRow tableRow42 = CreateTableRow(8, 12, 206);
            LotteryTableRow tableRow43 = CreateTableRow(18, 12, 217);
            LotteryTableRow tableRow44 = CreateTableRow(28, 12, 227);

            List<LotteryTableRow> table4Rows = new List<LotteryTableRow>();
            table4Rows.Add(tableRow41);
            table4Rows.Add(tableRow42);
            table4Rows.Add(tableRow43);
            table4Rows.Add(tableRow44);

            LotteryTablePosition table4Pos = CreateTablePosition(4, table4Rows);
            _Tables.Add(table4Pos);

            //Adding table 5
            LotteryTableRow tableRow51 = CreateTableRow(1, 67, 239);
            LotteryTableRow tableRow52 = CreateTableRow(8, 12, 249);
            LotteryTableRow tableRow53 = CreateTableRow(18, 12, 260);
            LotteryTableRow tableRow54 = CreateTableRow(28, 12, 270);

            List<LotteryTableRow> table5Rows = new List<LotteryTableRow>();
            table5Rows.Add(tableRow51);
            table5Rows.Add(tableRow52);
            table5Rows.Add(tableRow53);
            table5Rows.Add(tableRow54);

            LotteryTablePosition table5Pos = CreateTablePosition(5, table5Rows);
            _Tables.Add(table5Pos);

            //Adding table 6
            LotteryTableRow tableRow61 = CreateTableRow(1, 66, 281);
            LotteryTableRow tableRow62 = CreateTableRow(8, 12, 291);
            LotteryTableRow tableRow63 = CreateTableRow(18, 12, 301);
            LotteryTableRow tableRow64 = CreateTableRow(28, 12, 312);

            List<LotteryTableRow> table6Rows = new List<LotteryTableRow>();
            table6Rows.Add(tableRow61);
            table6Rows.Add(tableRow62);
            table6Rows.Add(tableRow63);
            table6Rows.Add(tableRow64);

            LotteryTablePosition table6Pos = CreateTablePosition(6, table6Rows);
            _Tables.Add(table6Pos);

            ////Adding table 7
            LotteryTableRow tableRow71 = CreateTableRow(1, 67, 323);
            LotteryTableRow tableRow72 = CreateTableRow(8, 12, 331);
            LotteryTableRow tableRow73 = CreateTableRow(18, 12, 344);
            LotteryTableRow tableRow74 = CreateTableRow(28, 12, 354);

            List<LotteryTableRow> table7Rows = new List<LotteryTableRow>();
            table7Rows.Add(tableRow71);
            table7Rows.Add(tableRow72);
            table7Rows.Add(tableRow73);
            table7Rows.Add(tableRow74);

            LotteryTablePosition table7Pos = CreateTablePosition(7, table7Rows);
            _Tables.Add(table7Pos);

            ////Adding table 8
            LotteryTableRow tableRow81 = CreateTableRow(1, 68, 368);
            LotteryTableRow tableRow82 = CreateTableRow(8, 14, 378);
            LotteryTableRow tableRow83 = CreateTableRow(18, 14, 389);
            LotteryTableRow tableRow84 = CreateTableRow(28, 14, 399);

            List<LotteryTableRow> table8Rows = new List<LotteryTableRow>();
            table8Rows.Add(tableRow81);
            table8Rows.Add(tableRow82);
            table8Rows.Add(tableRow83);
            table8Rows.Add(tableRow84);

            LotteryTablePosition table8Pos = CreateTablePosition(8, table8Rows);
            _Tables.Add(table8Pos);

            ////Adding table 9
            LotteryTableRow tableRow91 = CreateTableRow(1, 68, 409);
            LotteryTableRow tableRow92 = CreateTableRow(8, 14, 420);
            LotteryTableRow tableRow93 = CreateTableRow(18, 14, 430);
            LotteryTableRow tableRow94 = CreateTableRow(28, 14, 440);

            List<LotteryTableRow> table9Rows = new List<LotteryTableRow>();
            table9Rows.Add(tableRow91);
            table9Rows.Add(tableRow92);
            table9Rows.Add(tableRow93);
            table9Rows.Add(tableRow94);

            LotteryTablePosition table9Pos = CreateTablePosition(9, table9Rows);
            _Tables.Add(table9Pos);

            //Adding table 10
            LotteryTableRow tableRow101 = CreateTableRow(1, 68, 452);
            LotteryTableRow tableRow102 = CreateTableRow(8, 14, 462);
            LotteryTableRow tableRow103 = CreateTableRow(18, 14, 473);
            LotteryTableRow tableRow104 = CreateTableRow(28, 14, 483);

            List<LotteryTableRow> table10Rows = new List<LotteryTableRow>();
            table10Rows.Add(tableRow101);
            table10Rows.Add(tableRow102);
            table10Rows.Add(tableRow103);
            table10Rows.Add(tableRow104);

            LotteryTablePosition table10Pos = CreateTablePosition(10, table10Rows);
            _Tables.Add(table10Pos);

            //Adding table 11
            LotteryTableRow tableRow111 = CreateTableRow(1, 68, 493);
            LotteryTableRow tableRow112 = CreateTableRow(8, 14, 503);
            LotteryTableRow tableRow113 = CreateTableRow(18, 14, 514);
            LotteryTableRow tableRow114 = CreateTableRow(28, 14, 524);

            List<LotteryTableRow> table11Rows = new List<LotteryTableRow>();
            table11Rows.Add(tableRow111);
            table11Rows.Add(tableRow112);
            table11Rows.Add(tableRow113);
            table11Rows.Add(tableRow114);

            LotteryTablePosition table11Pos = CreateTablePosition(11, table11Rows);
            _Tables.Add(table11Pos);

            //Adding table 12
            LotteryTableRow tableRow121 = CreateTableRow(1, 68, 536);
            LotteryTableRow tableRow122 = CreateTableRow(8, 14, 546);
            LotteryTableRow tableRow123 = CreateTableRow(18, 14, 557);
            LotteryTableRow tableRow124 = CreateTableRow(28, 14, 567);

            List<LotteryTableRow> table12Rows = new List<LotteryTableRow>();
            table12Rows.Add(tableRow121);
            table12Rows.Add(tableRow122);
            table12Rows.Add(tableRow123);
            table12Rows.Add(tableRow124);

            LotteryTablePosition table12Pos = CreateTablePosition(12, table12Rows);
            _Tables.Add(table12Pos);

            //Adding table 13
            LotteryTableRow tableRow131 = CreateTableRow(1, 68, 577);
            LotteryTableRow tableRow132 = CreateTableRow(8, 14, 587);
            LotteryTableRow tableRow133 = CreateTableRow(18, 14, 598);
            LotteryTableRow tableRow134 = CreateTableRow(28, 14, 608);

            List<LotteryTableRow> table13Rows = new List<LotteryTableRow>();
            table13Rows.Add(tableRow131);
            table13Rows.Add(tableRow132);
            table13Rows.Add(tableRow133);
            table13Rows.Add(tableRow134);

            LotteryTablePosition table13Pos = CreateTablePosition(13, table13Rows);
            _Tables.Add(table13Pos);

            //Adding table 14
            LotteryTableRow tableRow141 = CreateTableRow(1, 68, 619);
            LotteryTableRow tableRow142 = CreateTableRow(8, 14, 629);
            LotteryTableRow tableRow143 = CreateTableRow(18, 14, 640);
            LotteryTableRow tableRow144 = CreateTableRow(28, 14, 650);

            List<LotteryTableRow> table14Rows = new List<LotteryTableRow>();
            table14Rows.Add(tableRow141);
            table14Rows.Add(tableRow142);
            table14Rows.Add(tableRow143);
            table14Rows.Add(tableRow144);

            LotteryTablePosition table14Pos = CreateTablePosition(14, table14Rows);
            _Tables.Add(table14Pos);

            ////Adding table 1
            //LotteryTableRow tableRow11 = CreateTableRow(1, 68, 67);
            //LotteryTableRow tableRow12 = CreateTableRow(8, 14, 78);
            //LotteryTableRow tableRow13 = CreateTableRow(18, 14, 88);
            //LotteryTableRow tableRow14 = CreateTableRow(28, 14, 99);

            //List<LotteryTableRow> table1Rows = new List<LotteryTableRow>();
            //table1Rows.Add(tableRow11);
            //table1Rows.Add(tableRow12);
            //table1Rows.Add(tableRow13);
            //table1Rows.Add(tableRow14);

            //LotteryTablePosition table1Pos = CreateTablePosition(1, table1Rows);
            //_Tables.Add(table1Pos);

            //////Adding table 2
            //LotteryTableRow tableRow21 = CreateTableRow(1, 68, 108);
            //LotteryTableRow tableRow22 = CreateTableRow(8, 14, 119);
            //LotteryTableRow tableRow23 = CreateTableRow(18, 14, 129);
            //LotteryTableRow tableRow24 = CreateTableRow(28, 14, 140);

            //List<LotteryTableRow> table2Rows = new List<LotteryTableRow>();
            //table2Rows.Add(tableRow21);
            //table2Rows.Add(tableRow22);
            //table2Rows.Add(tableRow23);
            //table2Rows.Add(tableRow24);

            //LotteryTablePosition table2Pos = CreateTablePosition(2, table2Rows);
            //_Tables.Add(table2Pos);

            ////Adding table 3
            //LotteryTableRow tableRow31 = CreateTableRow(1, 68, 151);
            //LotteryTableRow tableRow32 = CreateTableRow(8, 14, 162);
            //LotteryTableRow tableRow33 = CreateTableRow(18, 14, 172);
            //LotteryTableRow tableRow34 = CreateTableRow(28, 14, 183);

            //List<LotteryTableRow> table3Rows = new List<LotteryTableRow>();
            //table3Rows.Add(tableRow31);
            //table3Rows.Add(tableRow32);
            //table3Rows.Add(tableRow33);
            //table3Rows.Add(tableRow34);

            //LotteryTablePosition table3Pos = CreateTablePosition(3, table3Rows);
            //_Tables.Add(table3Pos);

            //////Adding table 4
            //LotteryTableRow tableRow41 = CreateTableRow(1, 68, 192);
            //LotteryTableRow tableRow42 = CreateTableRow(8, 14, 203);
            //LotteryTableRow tableRow43 = CreateTableRow(18, 14, 213);
            //LotteryTableRow tableRow44 = CreateTableRow(28, 14, 224);

            //List<LotteryTableRow> table4Rows = new List<LotteryTableRow>();
            //table4Rows.Add(tableRow41);
            //table4Rows.Add(tableRow42);
            //table4Rows.Add(tableRow43);
            //table4Rows.Add(tableRow44);

            //LotteryTablePosition table4Pos = CreateTablePosition(4, table4Rows);
            //_Tables.Add(table4Pos);

            ////Adding table 5
            //LotteryTableRow tableRow51 = CreateTableRow(1, 68, 235);
            //LotteryTableRow tableRow52 = CreateTableRow(8, 14, 245);
            //LotteryTableRow tableRow53 = CreateTableRow(18, 14, 256);
            //LotteryTableRow tableRow54 = CreateTableRow(28, 14, 266);

            //List<LotteryTableRow> table5Rows = new List<LotteryTableRow>();
            //table5Rows.Add(tableRow51);
            //table5Rows.Add(tableRow52);
            //table5Rows.Add(tableRow53);
            //table5Rows.Add(tableRow54);

            //LotteryTablePosition table5Pos = CreateTablePosition(5, table5Rows);
            //_Tables.Add(table5Pos);

            ////Adding table 6
            //LotteryTableRow tableRow61 = CreateTableRow(1, 68, 277);
            //LotteryTableRow tableRow62 = CreateTableRow(8, 14, 287);
            //LotteryTableRow tableRow63 = CreateTableRow(18, 14, 298);
            //LotteryTableRow tableRow64 = CreateTableRow(28, 14, 308);

            //List<LotteryTableRow> table6Rows = new List<LotteryTableRow>();
            //table6Rows.Add(tableRow61);
            //table6Rows.Add(tableRow62);
            //table6Rows.Add(tableRow63);
            //table6Rows.Add(tableRow64);

            //LotteryTablePosition table6Pos = CreateTablePosition(6, table6Rows);
            //_Tables.Add(table6Pos);

            //////Adding table 7
            //LotteryTableRow tableRow71 = CreateTableRow(1, 68, 319);
            //LotteryTableRow tableRow72 = CreateTableRow(8, 14, 329);
            //LotteryTableRow tableRow73 = CreateTableRow(18, 14, 340);
            //LotteryTableRow tableRow74 = CreateTableRow(28, 14, 350);

            //List<LotteryTableRow> table7Rows = new List<LotteryTableRow>();
            //table7Rows.Add(tableRow71);
            //table7Rows.Add(tableRow72);
            //table7Rows.Add(tableRow73);
            //table7Rows.Add(tableRow74);

            //LotteryTablePosition table7Pos = CreateTablePosition(7, table7Rows);
            //_Tables.Add(table7Pos);

            //////Adding table 8
            //LotteryTableRow tableRow81 = CreateTableRow(1, 68, 368);
            //LotteryTableRow tableRow82 = CreateTableRow(8, 14, 378);
            //LotteryTableRow tableRow83 = CreateTableRow(18, 14, 389);
            //LotteryTableRow tableRow84 = CreateTableRow(28, 14, 399);

            //List<LotteryTableRow> table8Rows = new List<LotteryTableRow>();
            //table8Rows.Add(tableRow81);
            //table8Rows.Add(tableRow82);
            //table8Rows.Add(tableRow83);
            //table8Rows.Add(tableRow84);

            //LotteryTablePosition table8Pos = CreateTablePosition(8, table8Rows);
            //_Tables.Add(table8Pos);

            //////Adding table 9
            //LotteryTableRow tableRow91 = CreateTableRow(1, 68, 409);
            //LotteryTableRow tableRow92 = CreateTableRow(8, 14, 420);
            //LotteryTableRow tableRow93 = CreateTableRow(18, 14, 430);
            //LotteryTableRow tableRow94 = CreateTableRow(28, 14, 440);

            //List<LotteryTableRow> table9Rows = new List<LotteryTableRow>();
            //table9Rows.Add(tableRow91);
            //table9Rows.Add(tableRow92);
            //table9Rows.Add(tableRow93);
            //table9Rows.Add(tableRow94);

            //LotteryTablePosition table9Pos = CreateTablePosition(9, table9Rows);
            //_Tables.Add(table9Pos);

            ////Adding table 10
            //LotteryTableRow tableRow101 = CreateTableRow(1, 68, 452);
            //LotteryTableRow tableRow102 = CreateTableRow(8, 14, 462);
            //LotteryTableRow tableRow103 = CreateTableRow(18, 14, 473);
            //LotteryTableRow tableRow104 = CreateTableRow(28, 14, 483);

            //List<LotteryTableRow> table10Rows = new List<LotteryTableRow>();
            //table10Rows.Add(tableRow101);
            //table10Rows.Add(tableRow102);
            //table10Rows.Add(tableRow103);
            //table10Rows.Add(tableRow104);

            //LotteryTablePosition table10Pos = CreateTablePosition(10, table10Rows);
            //_Tables.Add(table10Pos);

            ////Adding table 11
            //LotteryTableRow tableRow111 = CreateTableRow(1, 68, 493);
            //LotteryTableRow tableRow112 = CreateTableRow(8, 14, 503);
            //LotteryTableRow tableRow113 = CreateTableRow(18, 14, 514);
            //LotteryTableRow tableRow114 = CreateTableRow(28, 14, 524);

            //List<LotteryTableRow> table11Rows = new List<LotteryTableRow>();
            //table11Rows.Add(tableRow111);
            //table11Rows.Add(tableRow112);
            //table11Rows.Add(tableRow113);
            //table11Rows.Add(tableRow114);

            //LotteryTablePosition table11Pos = CreateTablePosition(11, table11Rows);
            //_Tables.Add(table11Pos);

            ////Adding table 12
            //LotteryTableRow tableRow121 = CreateTableRow(1, 68, 536);
            //LotteryTableRow tableRow122 = CreateTableRow(8, 14, 546);
            //LotteryTableRow tableRow123 = CreateTableRow(18, 14, 557);
            //LotteryTableRow tableRow124 = CreateTableRow(28, 14, 567);

            //List<LotteryTableRow> table12Rows = new List<LotteryTableRow>();
            //table12Rows.Add(tableRow121);
            //table12Rows.Add(tableRow122);
            //table12Rows.Add(tableRow123);
            //table12Rows.Add(tableRow124);

            //LotteryTablePosition table12Pos = CreateTablePosition(12, table12Rows);
            //_Tables.Add(table12Pos);

            ////Adding table 13
            //LotteryTableRow tableRow131 = CreateTableRow(1, 68, 577);
            //LotteryTableRow tableRow132 = CreateTableRow(8, 14, 587);
            //LotteryTableRow tableRow133 = CreateTableRow(18, 14, 598);
            //LotteryTableRow tableRow134 = CreateTableRow(28, 14, 608);

            //List<LotteryTableRow> table13Rows = new List<LotteryTableRow>();
            //table13Rows.Add(tableRow131);
            //table13Rows.Add(tableRow132);
            //table13Rows.Add(tableRow133);
            //table13Rows.Add(tableRow134);

            //LotteryTablePosition table13Pos = CreateTablePosition(13, table13Rows);
            //_Tables.Add(table13Pos);

            ////Adding table 14
            //LotteryTableRow tableRow141 = CreateTableRow(1, 68, 619);
            //LotteryTableRow tableRow142 = CreateTableRow(8, 14, 629);
            //LotteryTableRow tableRow143 = CreateTableRow(18, 14, 640);
            //LotteryTableRow tableRow144 = CreateTableRow(28, 14, 650);

            //List<LotteryTableRow> table14Rows = new List<LotteryTableRow>();
            //table14Rows.Add(tableRow141);
            //table14Rows.Add(tableRow142);
            //table14Rows.Add(tableRow143);
            //table14Rows.Add(tableRow144);

            //LotteryTablePosition table14Pos = CreateTablePosition(14, table14Rows);
            //_Tables.Add(table14Pos);
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
                    //if (iTable[i] < 4)
                    //{
                    //    x = row.StartingX + _CellHorizontalGap * (iTable[i] - row.RowNumber) - ((iTable[i] - row.RowNumber) / 3);
                    //}
                    //else if (iTable[i] < 14)
                    //{
                    //    x = row.StartingX + _CellHorizontalGap * (iTable[i] - row.RowNumber) - ((iTable[i] - row.RowNumber) / 3);
                    //}
                    //else if(iTable[i] < 24)
                    //{
                    //    x = row.StartingX + _CellHorizontalGap * (iTable[i] - row.RowNumber) - ((iTable[i] - row.RowNumber) / 3);
                    //}
                    //else if(iTable[i] < 24)
                    //{
                    //    x = row.StartingX + _CellHorizontalGap * (iTable[i] - row.RowNumber) - ((iTable[i] - row.RowNumber) / 3);
                    //}
                    //else if (iTable[i] < 34)
                    //{
                    //    x = row.StartingX + _CellHorizontalGap * (iTable[i] - row.RowNumber) - ((iTable[i] - row.RowNumber) / 3);
                    //}
                    //else
                    //{
                    //    x = row.StartingX + (_CellHorizontalGap * (iTable[i] - row.RowNumber)) + 1;
                    //}
                    x = GetXPositionForCell(iTable[i], row);

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

        private int GetXPositionForCell(int iNumber, LotteryTableRow iRow)
        {
            bool isSet = false;
            int res = 0;

            if (iNumber < 4)
            {
                res = iRow.StartingX + _CellHorizontalGap * (iNumber - iRow.RowNumber) - ((iNumber - iRow.RowNumber) / 3);
                isSet = true;
            }

            if (iNumber < 14 && !isSet)
            {
                res = iRow.StartingX + _CellHorizontalGap * (iNumber - iRow.RowNumber) - ((iNumber - iRow.RowNumber) / 3);
                //isSet = true;
            }

            if (iNumber < 24 && !isSet)
            {
                res = iRow.StartingX + _CellHorizontalGap * (iNumber - iRow.RowNumber) - ((iNumber - iRow.RowNumber) / 3);
                //isSet = true;
            }

            if (iNumber < 34 && !isSet)
            {
                res = iRow.StartingX + _CellHorizontalGap * (iNumber - iRow.RowNumber) - ((iNumber - iRow.RowNumber) / 3);
                //isSet = true;
            }

            if (!isSet)
            {
                res = iRow.StartingX + (_CellHorizontalGap * (iNumber - iRow.RowNumber)) + 1;
            }

            return res;
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
