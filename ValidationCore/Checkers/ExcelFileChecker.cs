using System.Collections.Generic;
using ValidationLib.ExcelValidation;

namespace ValidationLib.Checkers
{
    public static class ExcelFileChecker
    {
        private static ValidationCollection _validationCollection;

        public static bool IsExcelFileValid(string excelFileName)
        {
            _validationCollection = new ValidationCollection();
            var validations = new List<IValidation>();
            var isFileTypeValid = new CheckExcelFileType(excelFileName);
            var isnotNullOrEmpty = new CheckIfNotNullOrEmptyOrWhiteSpace(excelFileName);

            validations.Add(isFileTypeValid);
            validations.Add(isnotNullOrEmpty);

            _validationCollection.Validations = validations;
            return _validationCollection.IsValid;
        }

        private static bool IsValid
        {
            get { return _validationCollection.IsValid; }
        }
        
        private static IEnumerable<string> Message
        {
            get { return _validationCollection.Messages; }
        }
    }
}
