using ValidationLib.ExcelValidation;

namespace ValidationLib.Checkers
{
    public static class RecordChecker
    {
        private static IValidation _validation;
        public static bool IsFieldValid(string record)
        {
            if (record == null)
            {
                return false;   
            }

            _validation = new CheckIfNotNullOrEmptyOrWhiteSpace(record);
            return _validation.IsValid;
        }

        private static string Message
        {
            get { return _validation.Message; }
        }
    }
}
