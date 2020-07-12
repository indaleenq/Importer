using Importer;
using System;

namespace TesterProgram
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var file = @"\\MPH-LT-A50328\Users\A50328\Desktop\Employee_Upload_File-All.xlsx";
            var file = @"\\10.164.2.145\New_folder\Employee_Upload_File-All";
            var connectionString = "Server=MPH-VM-DBQ02;Database=MPHLtcms;user id=mphlis_dev;password=MPHLDEV2018!;";
            //LOCAL var connectionString = "Data Source=.;Initial Catalog=MPHLtcmsQS;Integrated Security=True;";
            ImportExcel importer = new ImportExcel(file, connectionString);
            importer.Import("Employees", "BadgeNumber");
            importer.Import("PeoplesoftPayroll", "BadgeNumber");

            if (string.IsNullOrEmpty(importer.ErrorMessage))
            {
                foreach (var records in importer.recordsResult.Records)
                {
                    Console.Write($"BadgeNumber = {importer.GetRecordValue(records, "BadgeNumber")} | ");
                    Console.Write($"{importer.GetTableName(records)} | ");
                    Console.WriteLine($"{importer.GetSaveMode(records)}");
                }
            }
        }
    }
}
