using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace ArdaDbMgr.Services
{
    public class DatabaseServices
    {
        const string TABLE_SCHEMA_HISTORY = "[_SchemaHistory_]";

        private readonly string _connectionString;

        public DatabaseServices(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateDatabase(string databaseName)
        {
            Validate(databaseName);

            // may have SQL injection!
            Execute($"CREATE DATABASE {databaseName}");
        }

        public void DropDatabase(string databaseName)
        {
            Validate(databaseName);

            // may have SQL injection!
            Execute($"DROP DATABASE {databaseName}");
        }

        public bool VerifyDatabase(string databaseName)
        {
            Validate(databaseName);

            var listDatabaseWithName = DatabaseCommand
                .Text($"SELECT name FROM sys.databases WHERE name=@dbname")
                .Parameter("@dbname", databaseName);

            var databaseFound = Execute(listDatabaseWithName, r => r["name"] );

            return (databaseFound.Count() > 0);
        }

        public void CreateSchemaHistory()
        {
            const string _SchemaHistory_ = TABLE_SCHEMA_HISTORY;

            var createTable = DatabaseCommand.Text(
                $"CREATE TABLE {_SchemaHistory_} (" +
                "  Seq INT PRIMARY KEY IDENTITY(1,1)," +
                "  Name NVARCHAR(256) NOT NULL," +
                "  Hash INT NULL" +
                ")");

            Execute(createTable);
        }

        void Validate(string name)
        {
            if (name == null)
                throw new ArgumentNullException("DatabaseServices: name is null");

            string objectName = name.Trim();

            if (objectName == "" || objectName == "[]")
                throw new ArgumentException("DatabaseServices: name is empty");

            if (objectName.Contains(";"))
                throw new InvalidOperationException("DatabaseServices: invalid name");

            if (objectName.Contains(" ") 
                || objectName.Contains("\t")
                || objectName.Contains("\r")
                || objectName.Contains("\n"))
                    throw new InvalidOperationException("DatabaseServices: name contains space");
        }

        void Execute(string commandText)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(commandText, connection);

                connection.Open();

                cmd.ExecuteNonQuery();
            }
        }

        void Execute(DatabaseCommand command)
        {
            Execute<object>(command);
        }

        T Execute<T>(DatabaseCommand command)
        {
            T result;

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = command.GetSqlCommand();

                connection.Open();

                cmd.Connection = connection;

                result = (T)cmd.ExecuteScalar();
            }

            return result;
        }

        IEnumerable<T> Execute<T>(DatabaseCommand command, Func<IDataRecord,T> getResult)
        {
            List<T> resultSet = new List<T>();

            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = command.GetSqlCommand();

                // open connection
                cmd.Connection = connection;
                connection.Open();

                // execute query
                var reader = cmd.ExecuteReader();

                // transform the DataReader to ResultSet<T>
                while(reader.Read())
                {
                    var result = getResult(reader);

                    resultSet.Add(result);
                }
            }

            return resultSet;
        }

        class DatabaseCommand
        {
            SqlCommand _command;

            public DatabaseCommand(string commandText)
            {
                _command = new SqlCommand(commandText);
            }

            public static DatabaseCommand Text(string commandText)
            {
                return new DatabaseCommand(commandText);
            }

            public DatabaseCommand Parameter(string parameterName, object value)
            {
                _command.Parameters.AddWithValue(parameterName, value);

                return this;
            }

            public SqlCommand GetSqlCommand()
            {
                return _command;
            }
        }
    }
}
