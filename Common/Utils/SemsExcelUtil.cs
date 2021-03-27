using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.SS.Util;

namespace Common.Utils
{
    public class SemsExcelUtil
    {
        public static void InsertRow(HSSFSheet _sheet, int fromRowIndex, int rowCount)
        {
            _sheet.ShiftRows(fromRowIndex, _sheet.LastRowNum, rowCount, true, false, true);
            for (int rowIndex = fromRowIndex; rowIndex < fromRowIndex + rowCount; rowIndex++)
            {
                HSSFRow rowSource = (HSSFRow)_sheet.GetRow(rowIndex + rowCount);
                HSSFRow rowInsert = (HSSFRow)_sheet.CreateRow(rowIndex);
                rowInsert.Height = rowSource.Height;
                for (int colIndex = 0; colIndex < rowSource.LastCellNum; colIndex++)
                {
                    HSSFCell cellSource = (HSSFCell)rowSource.GetCell(colIndex);
                    HSSFCell cellInsert = (HSSFCell)rowInsert.CreateCell(colIndex);
                    if (cellSource != null)
                    {
                        cellInsert.CellStyle = cellSource.CellStyle;
                    }
                }
            }
        }

        public static void CopyRow(XSSFSheet _worksheet, int _sourceRowNumStart, int _sourceCount, int _destinationRowNum)
        {
            XSSFRow sourceRow = null;
            XSSFRow newRow = null;
            XSSFCell cellSource = null;
            XSSFCell cellInsert = null;
            Dictionary<int, int> mRowNum = new Dictionary<int, int>();

            for (int index = _sourceRowNumStart; index < (_sourceRowNumStart + _sourceCount); index++)
            {
                sourceRow = (XSSFRow)_worksheet.GetRow(index);
                newRow = (XSSFRow)_worksheet.CreateRow(_destinationRowNum + (index - _sourceRowNumStart));

                newRow.Height = sourceRow.Height;
                for (int colIndex = 0; colIndex < sourceRow.LastCellNum; colIndex++)
                {
                    cellSource = (XSSFCell)sourceRow.GetCell(colIndex);
                    cellInsert = (XSSFCell)newRow.CreateCell(colIndex);
                    if (cellSource != null)
                    {
                        cellInsert.CellStyle = cellSource.CellStyle;
                        cellInsert.SetCellType(cellSource.CellType);
                    }

                }
                mRowNum.Add(sourceRow.RowNum, newRow.RowNum);
            }

            for (int i = 0; i < _worksheet.NumMergedRegions; i++)
            {
                CellRangeAddress cellRangeAddress = _worksheet.GetMergedRegion(i);
                if (mRowNum.ContainsKey(cellRangeAddress.FirstRow))
                {
                    int newRowNum = mRowNum[cellRangeAddress.FirstRow];
                    CellRangeAddress newCellRangeAddress = new CellRangeAddress(newRowNum,
                                                                                (newRowNum +
                                                                                (cellRangeAddress.LastRow -
                                                                                cellRangeAddress.FirstRow)),
                                                                                cellRangeAddress.FirstColumn,
                                                                                cellRangeAddress.LastColumn);
                    _worksheet.AddMergedRegion(newCellRangeAddress);
                }
            }
        }

        public static void CopyCols(XSSFSheet _worksheet, int _sourceRowNumStart, int _sourceRowNumEnd, int _sourceColNumStart, int _sourceColNumEnd, int _destinationColNum)
        {
            XSSFCell cellSource = null;
            XSSFCell cellInsert = null;

            for (int colIndex = _sourceColNumStart; colIndex <= _sourceColNumEnd; colIndex++)
            {
                for (int rowIndex = _sourceRowNumStart; rowIndex <= _sourceRowNumEnd; rowIndex++ )
                {
                    cellSource = (XSSFCell)_worksheet.GetRow(rowIndex).GetCell(colIndex);

                    for (int destination = 1; destination < _destinationColNum; destination++)
                    {
                        cellInsert = (XSSFCell)_worksheet.GetRow(rowIndex).CreateCell(colIndex + destination);
                        _worksheet.SetColumnWidth(colIndex + destination, (short)600);
                        if (cellSource != null)
                        {
                            cellInsert.CellComment = cellSource.CellComment;
                            cellInsert.CellStyle = cellSource.CellStyle;
                            cellInsert.Hyperlink = cellSource.Hyperlink;
                            switch (cellSource.CellType)
                            {
                                case CellType.Formula:
                                    string tmpVal = "";
                                    if (cellSource.CellFormula.IndexOf("CHOOSE") > -1)
                                    {
                                        tmpVal = string.Format("CHOOSE(WEEKDAY({0}5,1),\"일\",\"월\",\"화\",\"수\",\"목\",\"금\",\"토\")", CellReference.ConvertNumToColString(colIndex + destination));
                                    }
                                    else if (cellSource.CellFormula.IndexOf("$D$4") > -1)
                                    {
                                        tmpVal = string.Format("$D$4+{0}", destination);
                                    }
                                    else
                                    {
                                        tmpVal = cellSource.CellFormula;
                                    }

                                    cellInsert.CellFormula = tmpVal; 
                                    break;
                                case CellType.Numeric:
                                    cellInsert.SetCellValue(cellSource.NumericCellValue); break;
                                case CellType.String:
                                    cellInsert.SetCellValue(cellSource.StringCellValue); break;
                            }

                            if (cellSource.IsMergedCell && (cellSource.CellType == CellType.Formula && cellSource.CellFormula != null && cellSource.CellFormula.Length > 0))
                            {
                                CellRangeAddress newCellRangeAddress = new CellRangeAddress(rowIndex, rowIndex + 1, colIndex + destination, colIndex + destination);
                                _worksheet.AddMergedRegion(newCellRangeAddress);
                            }
                        }
                    }
                }
            }
        }

        public static Dictionary<string, Dictionary<string, int>> getExcelLevel()
        {
            Dictionary<string, Dictionary<string, int>> main = new Dictionary<string, Dictionary<string, int>>();
            for (int i = 1; i < 16; i++)
            {
                Dictionary<string, int> sub = new Dictionary<string, int>();
                sub.Add("POS", i + 1);
                sub.Add("COUNT", 0);
                sub.Add("pIdx", 0);
                main.Add(Convert.ToString(i), sub);
            }
            return main;
        }
    }
}
