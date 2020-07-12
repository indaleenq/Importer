using System.Collections.Generic;

namespace Importer
{
    public class ImportModel
    {
        public List<string> FieldNames = new List<string>();

        public List<List<Record>> Records = new List<List<Record>>();
        
    }

    public class Record
    {
        public string RecordField { get; set; }

        public string RecordValue { get; set; }

        public List<string> Errors { get; set; }

        public bool IsValid { get; set; }

        public SaveMode SaveMode { get; set; }

        public string PrimaryKey { get; set; }

        public string TableName { get; set; }
    }
}
