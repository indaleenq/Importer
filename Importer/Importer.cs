using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Importer
{
    public class Importer : IImporter
    {
        private string _targetFileName;
        private string _destinationDatabase;
        protected string _primary;
        protected string _dbTableName;
        protected List<string> _dbFieldNames;
        public string ErrorMessage = string.Empty;
        public ImportModel recordsResult = new ImportModel();
        private SqlConnection dbConnection;

        public string TargetFileName
        {
            get
            {
                return _targetFileName;
            }
        }

        public string DestinationDb
        {
            get
            {
                return _destinationDatabase;
            }
        }

        protected Importer(string targetFileName, string destinationDatabase)
        {
            _targetFileName = targetFileName;
            _destinationDatabase = destinationDatabase;
            dbConnection = new SqlConnection(DestinationDb);
        }

        protected void SaveToDatabase(ImportModel model, SaveMode mode)
        {
            int rows = 0;
            try
            {
                foreach (var records in model.Records)
                {
                    var statement = GetSqlStatement(mode, records);
                    SqlCommand command = new SqlCommand(statement, dbConnection);
                    command.CommandType = CommandType.Text;

                    if (!(records.Exists(x => !x.IsValid)))
                    {
                        foreach (var record in records)
                        {
                            command.Parameters.AddWithValue($"@{record.RecordField}", record.RecordValue);
                        }
                        dbConnection.Open();
                        rows = command.ExecuteNonQuery();
                        dbConnection.Close();
                    }
                }
                if (rows > 0) { PopulateResults(model, mode); }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Encountered Error Importing file {_targetFileName}. Error Message: {ex.Message}";
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        private void PopulateResults(ImportModel model, SaveMode mode)
        {
            foreach (var records in model.Records)
            {
                foreach (var record in records)
                {
                    record.SaveMode = record.IsValid ? mode : SaveMode.NotImported;
                    record.PrimaryKey = _primary;
                    record.TableName = _dbTableName;
                }
                recordsResult.Records.Add(records);
            }
        }

        private string GetSqlStatement(SaveMode mode, List<Record> records)
        {
            switch (mode)
            {
                case SaveMode.Insert:
                    return CreateInsertStatement();
                case SaveMode.Update:
                    var statement = CreateUpdateStatement();
                    return statement.StartsWith("UPDATE") ? statement + $" WHERE {_primary} = '{GetPrimaryValue(records)}'" : statement;
                case SaveMode.Delete:
                    return CreateDeleteStatement(GetPrimaryValue(records));
                default:
                    return string.Empty;
            }
        }

        protected bool DeleteAllFromTable()
        {
            try
            {
                string deleteStatement = $"DELETE FROM {_dbTableName}";

                SqlCommand deleteCommand = new SqlCommand(deleteStatement, dbConnection);
                deleteCommand.CommandType = CommandType.Text;

                dbConnection.Open();
                int rows = deleteCommand.ExecuteNonQuery();
                dbConnection.Close();

                return rows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Encountered error deleting records. Error: {ex.Message}";
                return false;
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        protected string CreateInsertStatement()
        {
            var insertStatement = $"INSERT INTO {_dbTableName} (";
            string values = string.Empty;

            foreach (var fieldName in _dbFieldNames)
            {
                insertStatement = $"{insertStatement}[{fieldName}], ";
                values = $"{values}@{fieldName}, ";
            }

            insertStatement = insertStatement.Remove(insertStatement.Length - 2);
            values = values.Remove(values.Length - 2);
            return string.Concat($"{insertStatement})", $" VALUES ({values})");
        }

        protected string CreateUpdateStatement()
        {
            var updateStatement = $"UPDATE {_dbTableName} ";
            string setValues = string.Empty;

            foreach (var fieldName in _dbFieldNames)
            {
                setValues = $"{setValues}[{fieldName}] = @{fieldName}, ";
            }

            setValues = setValues.Remove(setValues.Length - 2);
            return string.Concat($"{updateStatement}", $" SET {setValues}");
        }

        protected string CreateDeleteStatement(string value)
        {
            return $"DELETE INTO {_dbTableName} WHERE {_primary} = '{value}'";
        }

        protected ImportModel GetAllRecordsInTable()
        {
            var existingDBdata = new ImportModel();
            List<Record> existingRecordPerRow;

            var selectAllStatement = $"SELECT * FROM {_dbTableName}";

            SqlCommand selectCommand = new SqlCommand(selectAllStatement, dbConnection);
            dbConnection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();

            var fieldNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

            while (reader.Read())
            {
                existingRecordPerRow = new List<Record>();

                for (int fieldCounter = 1; fieldCounter < fieldNames.Count; fieldCounter++)
                {
                    var recordFieldName = fieldNames[fieldCounter - 1];
                    existingRecordPerRow.Add(new Record
                    {
                        RecordField = recordFieldName,
                        RecordValue = Convert.ToString(reader[recordFieldName]),
                        IsValid = true
                    });
                }
                existingDBdata.Records.Add(existingRecordPerRow);
            }
            dbConnection.Close();
            return existingDBdata;
        }

        protected void UpdateInsertRecords(ImportModel toBeUploadRecords)
        {
            if (Validate())
            {
                var existingRecords = GetAllRecordsInTable();

                var forUpdateRecords = new ImportModel();
                var forNewRecords = new ImportModel();

                var toBeUniques = new List<Record>();
                var existUniques = new List<Record>();

                foreach (var upload in toBeUploadRecords.Records)
                {
                    toBeUniques.Add(new Record { RecordField = _primary, RecordValue = GetPrimaryValue(upload) });

                    foreach (var exists in existingRecords.Records)
                    {
                        existUniques.Add(new Record { RecordField = _primary, RecordValue = GetPrimaryValue(exists) });

                        if (exists.Exists(x => x.RecordValue == GetPrimaryValue(upload))) { forUpdateRecords.Records.Add(upload); break; }
                    }
                }

                var result = toBeUniques.Where(x => !existUniques.Any(y => y.RecordValue == x.RecordValue)).ToList();

                foreach (var upload in toBeUploadRecords.Records)
                {
                    foreach (var res in result)
                    {
                        if (upload.Exists(x => x.RecordValue == res.RecordValue)) { forNewRecords.Records.Add(upload); }
                    }
                }

                SaveToDatabase(forUpdateRecords, SaveMode.Update);
                SaveToDatabase(forNewRecords, SaveMode.Insert);
            }
            else
            {
                this.ErrorMessage = "FieldNames, TableName and PrimaryKey cannot be null or empty.";
            }
        }

        private string GetPrimaryValue(List<Record> records)
        {
            return records.Where(x => x.RecordField == _primary).ToList().FirstOrDefault().RecordValue;
        }

        public string GetRecordValue(List<Record> records, string fieldName)
        {
            return records.Where(x => x.RecordField == fieldName).ToList().FirstOrDefault().RecordValue;
        }

        public string GetSaveMode(List<Record> records)
        {
            return records.Where(x => x.RecordField == x.PrimaryKey).ToList().FirstOrDefault().SaveMode.ToString();
        }

        public string GetTableName(List<Record> records)
        {
            return records.Where(x => x.RecordField == x.PrimaryKey).ToList().FirstOrDefault().TableName;
        }

        private bool Validate()
        {
            return _dbFieldNames.Count != 0 && !string.IsNullOrEmpty(_dbTableName) && !string.IsNullOrEmpty(_primary) && !string.IsNullOrWhiteSpace(_primary);
        }
    }
}