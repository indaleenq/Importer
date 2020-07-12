using System;
using System.Collections.Generic;

namespace Importer
{
    public class ExcelFileValidator
    {
        public event ValidateDelegate Validations;
        static List<ValidateEventArgs> ValidationArgs = new List<ValidateEventArgs>();

        public void Validate(string excelFile)
        {
            ValidateEventArgs validationArg = new ValidateEventArgs();
            Validations(excelFile, validationArg);
        }

        public bool IsValidExcelFileType(object excelFile, ValidateEventArgs args)
        {
            if (excelFile.ToString().Contains(".xlsx") || excelFile.ToString().Contains(".xls"))
            {
                args.IsValid = true;
                args.Message = "Valid Excel Type";
            }

            return args.IsValid;
        }
    }

    public class MainValidator
    {
        public static bool IsThisValid(string excelFile)
        {
            ExcelFileValidator excelValidator = new ExcelFileValidator();
            excelValidator.Validations += GeneralValidator<string>.IsStringNullOrEmptyOrWhiteSpace;
            excelValidator.Validations += excelValidator.IsValidExcelFileType;
            return true;
        }

    }

    public static class GeneralValidator<T> where T: class
    {
        public static bool IsStringNullOrEmptyOrWhiteSpace(object toValidate, ValidateEventArgs args)
        {
            if (string.IsNullOrEmpty(toValidate.ToString()) || string.IsNullOrWhiteSpace(toValidate.ToString()))
            {
                args.IsValid = true;
                args.Message = $"{toValidate} is empty or null";

                return args.IsValid;
            }

            return args.IsValid;
        }

        public static bool IsNotNull(T toValidate)
        {
            return toValidate != null;
        }
    }

    public delegate bool ValidateDelegate(object sender, ValidateEventArgs args);

    public class ValidateEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }
    
}
