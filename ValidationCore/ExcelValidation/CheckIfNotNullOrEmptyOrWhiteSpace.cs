namespace ValidationLib.ExcelValidation
{
    public class CheckIfNotNullOrEmptyOrWhiteSpace : ValidationBase<string>
    {
        public CheckIfNotNullOrEmptyOrWhiteSpace(string context) : base(context)
        {
        }

        public override bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Context) && !string.IsNullOrWhiteSpace(Context);
            }
        }

        public override string Message
        {
            get
            {
                return $"{Context} cannot be null or empty or white space.";
            }
        }
    }
}
