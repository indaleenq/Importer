using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer
{
    public interface IImporter
    {
        string TargetFileName { get; }

        string DestinationDb { get; }
    }
}
