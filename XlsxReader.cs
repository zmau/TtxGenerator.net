using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;

//https://www.codeproject.com/KB/office/670141/OpenXMLExcel.zip

namespace TtxGenerator.net
{
    public class XlsxReader
    {
        private WorkbookPart? _workbookPart;
        private List<CoverageTypeRow> _rowList;
        private string _coverageTypeCode;
        private LobProfile _lobInputProfile;


        public XlsxReader()
        {
            _rowList = new List<CoverageTypeRow>();
        }
        public void FilterByCoverageType(string inputPath, string CoverageTypeCode, LobProfile lobInputProfile)
        {
            _coverageTypeCode = CoverageTypeCode;
            _lobInputProfile = lobInputProfile;
            string spreadsheetFullPath = $"{inputPath}\\{_lobInputProfile.SpreadsheetFileName}";
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(spreadsheetFullPath, false))
            {
                _workbookPart = spreadsheetDocument.WorkbookPart;
                List<Row> rows;
                try
                {
                    var sheets = _workbookPart.Workbook.Descendants<Sheet>();
                    var sheet = sheets.First(sh => sh.Name == _lobInputProfile.TabName);
                    if (sheet is null)
                    {
                        Console.WriteLine($"could not find sheet {_lobInputProfile.TabName} inside file {_lobInputProfile.SpreadsheetFileName}");
                        Console.ReadLine();
                        return;
                    }
                    var workSheet = ((WorksheetPart)_workbookPart.GetPartById(sheet.Id)).Worksheet;
                    var columns = workSheet.Descendants<Columns>().FirstOrDefault();

                    var sheetData = workSheet.Elements<SheetData>().First();
                    rows = sheetData.Elements<Row>().ToList();
                }
                catch (Exception e)
                {
                    return;
                }
                if (rows.Count > 1)
                {
                    for (var i = 1; i < rows.Count; i++)
                    {
                        var dataRow = new List<string>();
                        var row = rows[i];
                        if (rowHasCoverageOf(row, CoverageTypeCode))
                        {
                            _rowList.Add(newCoverageTypeRow(row));
                        }
                    }
                }
            }
        }

        public List<CoverageTypeRow> RowList { get { return _rowList; } }
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
        private bool rowHasCoverageOf(Row row, string coverageTypeCode)
        {
            foreach (Cell cell in row.Descendants<Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);
                if (columnName.Equals(_lobInputProfile.CoverageTypeCodeColumn))
                {
                    var coverageTypeCodeValue = ReadExcelCell(cell, _workbookPart);
                    return coverageTypeCodeValue == coverageTypeCode;
                }
            }
            return false;
            //throw new Exception("filter column not found!");
        }

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


    }
}
