using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationLib
{
    public interface IValidation
    {
        bool IsValid { get; }
        void Validate();
        string Message { get; }
    }
}
