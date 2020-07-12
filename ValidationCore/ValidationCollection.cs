using System.Collections.Generic;
using System.Linq;

namespace ValidationLib
{
    public class ValidationCollection : IValidationList
    {
        public List<IValidation> Validations;
        public bool IsValid
        {
            get
            {
                return this.Validations.All(x => x.IsValid);
            }
        }

        public IEnumerable<string> Messages
        {
            get
            {
                return this.Validations.Where(x => x.IsValid).Select(y => y.Message);
            }
        }

        public void Validate()
        {
            foreach (var validation in this.Validations)
            {
                validation.Validate();
            }
        }
    }
}
