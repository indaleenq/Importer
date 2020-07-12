using System.Collections.Generic;

namespace ValidationLib.ExcelValidation
{
    public class CheckExcelFileType : ValidationBase<string>
    {
        public static readonly List<string> validExcelFileType = new List<string>() {".xlsx", ".xls"};

        public CheckExcelFileType(string context) : base (context)
        {

        }

        public override bool IsValid
        {
            get
            {
                var valid = true;

                foreach (var item in validExcelFileType)
                {
                    valid = Context.Contains(item);
                }

                return valid;
            }
        }

        public override string Message
        {
            get
            {
                return $"{Context} is not a valid excel file type.";
            }
        }
    }
}
