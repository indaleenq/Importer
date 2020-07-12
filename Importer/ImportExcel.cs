using System.Collections.Generic;
using ValidationLib.Checkers;
using Excel = Microsoft.Office.Interop.Excel;

namespace Importer
{
    public class ImportExcel : Importer
    {
        private Excel.Application xlApp;
        private Excel.Workbook xlWorkBook;
        private Excel.Worksheet xlWorkSheet;
        private Excel.Range range;
        protected ImportModel model;

        public ImportExcel(string targetFileName, string destinationDatabase) : base(targetFileName, destinationDatabase)
        {
            model = new ImportModel();
        }

        public void Import(string workSheetName, string uniqueField)
        {
            if (ExcelFileChecker.IsExcelFileValid(TargetFileName) && !string.IsNullOrEmpty(DestinationDb))
            {
                GetExcelWorkSheetData(workSheetName);

                _dbTableName = xlWorkSheet.Name;
                _dbFieldNames = GetFieldNames(range);
                _primary = uniqueField;
                model.Records = GetRecordsFromExcel(range);

                ReleaseExcelResource();
                UpdateInsertRecords(model);
            }
        }

        private void ReleaseExcelResource()
        {
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
        }

        private void GetExcelWorkSheetData(int worksheetNumber)
        {
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(@"" + TargetFileName + "", 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(worksheetNumber);
            range = xlWorkSheet.UsedRange;
        }

        private void GetExcelWorkSheetData(string worksheetName)
        {
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(@"" + TargetFileName + "", 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(worksheetName);
            range = xlWorkSheet.UsedRange;
        }

        private List<List<Record>> GetRecordsFromExcel(Excel.Range range)
        {
            List<Record> recordPerRow;
            ImportModel model = new ImportModel();

            for (int rowCounter = 2; rowCounter <= range.Rows.Count; rowCounter++)
            {
                recordPerRow = new List<Record>();
                for (int colCounter = 1; colCounter <= range.Columns.Count; colCounter++)
                {
                    var value = (string)(range.Cells[rowCounter, colCounter] as Excel.Range).Value2;
                    recordPerRow.Add(new Record
                    {
                        RecordField = _dbFieldNames[colCounter - 1],
                        RecordValue = value,
                        IsValid = RecordChecker.IsFieldValid(value)
                    });
                }
                model.Records.Add(recordPerRow);
            }

            return model.Records;
        }

        private List<string> GetFieldNames(Excel.Range range)
        {
            List<string> fieldNames = new List<string>();

            for (int rowCounter = 1; rowCounter <= 1; rowCounter++)
            {
                for (int colCounter = 1; colCounter <= range.Columns.Count; colCounter++)
                {
                    fieldNames.Add((string)(range.Cells[rowCounter, colCounter] as Excel.Range).Value2);
                }
            }

            return fieldNames;
        }
    }
}
