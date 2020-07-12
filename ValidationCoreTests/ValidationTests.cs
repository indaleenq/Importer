using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ValidationLib.Checkers;

namespace ValidationCoreTests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void ExcelFileXLSIsValid()
        {
            var excelFileName = "SampleExcelFile.xls";
            var isExcelFileValid = ExcelFileChecker.IsExcelFileValid(excelFileName);

            Assert.AreEqual(true, isExcelFileValid);
        }

        [TestMethod]
        public void ExcelFileXLSXIsValid()
        {
            var excelFileName = "SampleExcelFile.xlsx";
            var isExcelFileValid = ExcelFileChecker.IsExcelFileValid(excelFileName);

            Assert.AreEqual(true, isExcelFileValid);
        }

        [TestMethod]
        public void ExcelFileIsNotValid()
        {
            var excelFileName = "SampleExcelFile.txt";
            var isExcelFileValid = ExcelFileChecker.IsExcelFileValid(excelFileName);

            Assert.AreEqual(false, isExcelFileValid);
        }

        [TestMethod]
        public void ExcelFileMustNotBeEmpty()
        {
            var excelFileName = string.Empty;
            var isExcelFileValid = ExcelFileChecker.IsExcelFileValid(excelFileName);

            Assert.AreEqual(false, isExcelFileValid);
        }

        [TestMethod]
        public void ExcelFileMustNotBeNull()
        {
            string excelFileName = null;
            var isExcelFileValid = ExcelFileChecker.IsExcelFileValid(excelFileName);

            Assert.AreEqual(false, isExcelFileValid);
        }
    }
}
