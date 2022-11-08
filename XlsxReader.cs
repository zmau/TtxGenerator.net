using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

//https://www.codeproject.com/KB/office/670141/OpenXMLExcel.zip

namespace TtxGenerator.net
{
    public class XlsxReader
    {
        private WorkbookPart? _workbookPart;
        private List<CoverageTypeRow> _lobMappingRowList;

        private string _coverageTypeCode;
        private LobProfile _lobInputProfile;
        private IEnumerable<Sheet> _sheets;

        #region common
        public XlsxReader(string inputPath, LobProfile lobInputProfile, string CoverageTypeCode)
        {
            _lobMappingRowList = new List<CoverageTypeRow>();
            _lobInputProfile = lobInputProfile;
            var spreadsheetFullPath = $"{inputPath}\\{lobInputProfile.SpreadsheetFileName}";
            _coverageTypeCode = CoverageTypeCode;
            try
            {
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(spreadsheetFullPath, false);
                _workbookPart = spreadsheetDocument.WorkbookPart;
                _sheets = _workbookPart.Workbook.Descendants<Sheet>();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Could not find the file {spreadsheetFullPath}");
                Console.ReadLine();
                Environment.Exit(1);
            }
            catch (IOException e)
            {
                Console.WriteLine($"Cannot read the file {spreadsheetFullPath}. {e.Message}");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        private List<Row> GetRowsBySheetName(string sheetName)
        {
            var sheet = _sheets.First(sh => sh.Name == sheetName);
            if (sheet is null)
            {
                Console.WriteLine($"could not find the sheet {sheetName} inside file {_lobInputProfile.SpreadsheetFileName}");
                Console.ReadLine();
                Environment.Exit(1);
            }
            var workSheet = ((WorksheetPart)_workbookPart.GetPartById(sheet.Id)).Worksheet;
            var sheetData = workSheet.Elements<SheetData>().First();
            return sheetData.Elements<Row>().ToList();
        }
        private bool rowHasCoverageOf(Row row, string coverageTypeCode, string coverageTypeColumnIndex)
        {
            foreach (Cell cell in row.Descendants<Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);
                if (columnName.Equals(coverageTypeColumnIndex))
                {
                    var coverageTypeCodeValue = ReadExcelCell(cell, _workbookPart);
                    return coverageTypeCodeValue == coverageTypeCode;
                }
            }
            return false;
            //throw new Exception("filter column not found!");
        }
        #endregion

        #region LOBMapping
        public void FilterByCoverageType()
        {
            List<Row> rows = GetRowsBySheetName(_lobInputProfile.LOBMappingTabName);
            try
            {
                for (var i = 1; i < rows.Count; i++)
                {
                    var dataRow = new List<string>();
                    var row = rows[i];
                    if (rowHasCoverageOf(row, _coverageTypeCode, _lobInputProfile.CoverageTypeCodeColumn))
                    {
                        _lobMappingRowList.Add(newCoverageTypeRow(row));
                    }
                }
                if (_lobMappingRowList.Count == 0)
                {
                    Console.WriteLine($"Could not find coverage type code {_coverageTypeCode} in spreadsheet!");
                    Console.ReadLine();
                    Environment.Exit(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception {e.Message}. Failed to read data.");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        public List<CoverageTypeRow> LobMappingRowList { get { return _lobMappingRowList; } }
        private CoverageTypeRow newCoverageTypeRow(Row row)
        {
            var cellEnumerator = GetExcelCellEnumerator(row);
            CoverageTypeRow newRow = new CoverageTypeRow();
            newRow.CoverageTypeCode = _coverageTypeCode;
            while (cellEnumerator.MoveNext())
            {
                var cell = cellEnumerator.Current;
                var text = ReadExcelCell(cell, _workbookPart).Trim();
                string columnName = GetColumnName(cell.CellReference);
                if(columnName.Equals(_lobInputProfile.PolicyTypeCodeColumn))
                    newRow.PolicyTypeCode = text;
                else if (columnName.Equals(_lobInputProfile.CoverageTypeNameColumn))
                    newRow.CoverageTypeName = text;
                else if (columnName.Equals(_lobInputProfile.CoverageTypeCodeColumn))
                    newRow.CoverageTypeCode = text;
                else if (columnName.Equals(_lobInputProfile.CoverageSubTypeNameColumn))
                    newRow.CoverageSubTypeName = text;
                else if (columnName.Equals(_lobInputProfile.CoverageSubTypeCodeColumn))
                    newRow.CoverageSubTypeCode = text;
                else if (columnName.Equals(_lobInputProfile.ExposureTypeCodeColumn))
                    newRow.ExposureTypeCode = text;
                else if (columnName.Equals(_lobInputProfile.CostCategoryCodeColumn))
                    newRow.CostCategoryCode = text;
                else if (columnName.Equals(_lobInputProfile.CostCategoryNameColumn))
                    newRow.CostCategoryName = text;
                else if (columnName.Equals(_lobInputProfile.CovTermCodeColumn))
                    newRow.CovTermCode = text;
            }
            return newRow;
        }
        #endregion

        #region ISOMapping
        public List<ISOMappingItem> ISOMappingList
        {
            get
            {
                List<ISOMappingItem> isoMappingList = new List<ISOMappingItem>();
                List<Row> rows = GetRowsBySheetName(_lobInputProfile.ISOMappingTabName);
                List<Row> rowsForCoverageType = rows.Where(r => rowHasCoverageOf(r, _coverageTypeCode, _lobInputProfile.CC_CoverageTypeColumn)).ToList();
                foreach(Row row in rowsForCoverageType)
                {
                    isoMappingList.Add(newISOMappingItem(row));
                }
                return isoMappingList;
            }
        }
        private ISOMappingItem newISOMappingItem(Row row)
        {
            var cellEnumerator = GetExcelCellEnumerator(row);
            ISOMappingItem isoMappingItem = new ISOMappingItem();
            isoMappingItem.CoverageType = _coverageTypeCode;
            while (cellEnumerator.MoveNext())
            {
                var cell = cellEnumerator.Current;
                var text = ReadExcelCell(cell, _workbookPart).Trim();
                string columnName = GetColumnName(cell.CellReference);
                if (columnName.Equals(_lobInputProfile.CC_CoverageSubTypeColumn))
                    isoMappingItem.CoverageSubtype = text;
                else if (columnName.Equals(_lobInputProfile.ISO_PolicyTypeColumn))
                    isoMappingItem.ISOPolicyType = text;
                else if (columnName.Equals(_lobInputProfile.ISO_CoverageTypeColumn))
                    isoMappingItem.ISOCoverageType = text;
                else if (columnName.Equals(_lobInputProfile.ISO_LossTypeColumn))
                    isoMappingItem.ISOLossType = text;
            }
            return isoMappingItem;
        }
        #endregion


        #region infrastructure
        private IEnumerator<Cell> GetExcelCellEnumerator(Row row)
        {
            int currentCount = 0;
            foreach (Cell cell in row.Descendants<Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);

                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    var emptycell = new Cell() { DataType = null, CellValue = new CellValue(string.Empty) };
                    yield return emptycell;
                }

                yield return cell;
                currentCount++;
            }
        }
        private string GetColumnName(string cellReference)
        {
            var regex = new Regex("[A-Za-z]+");
            var match = regex.Match(cellReference);

            return match.Value;
        }

        private int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            var convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                int current = i == 0 ? letter - 65 : letter - 64; // ASCII 'A' = 65
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }

        private string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var cellValue = cell.CellValue;
            var text = (cellValue == null) ? cell.InnerText : cellValue.Text;
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                text = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(
                        Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }
            return (text ?? string.Empty).Trim();
        }
        #endregion

    }
}
