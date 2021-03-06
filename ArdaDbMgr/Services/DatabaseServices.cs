﻿#define DISPOSE_CONNECTION

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using ArdaDbMgr.Services.Models;
using ArdaDbMgr.Models;

namespace ArdaDbMgr.Services
{
    public class DatabaseServices : IDatabaseServices, IDisposable
    {
        const string TABLE_SCHEMA_HISTORY = "[_SchemaHistory_]";

        private readonly string _connectionString;
        private SqlConnection _currentConnection = null;

        public DatabaseServices(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateDatabase(string databaseName)
        {
            // may have SQL injection!
            Validate(databaseName);

            var createDatabase = DatabaseCommand.Text
                ($"CREATE DATABASE {databaseName}");

            Execute(createDatabase);
        }

        public void DropDatabase(string databaseName)
        {
            // may have SQL injection!
            Validate(databaseName);

            var dropDatabase = DatabaseCommand.Text
                ($"DROP DATABASE {databaseName}");

            Execute(dropDatabase);
        }

        public bool VerifyDatabase(string databaseName)
        {
            Validate(databaseName);

            var listDatabaseWithName = DatabaseCommand.Text
                ($"SELECT DB_ID(@dbname)")
                .Parameter("@dbname", databaseName);

            var dbid = Execute<short?>(listDatabaseWithName);

            return (dbid > 0);
        }

        public string GetDatabaseName()
        {
            var getDatabaseName = DatabaseCommand.Text
                ($"SELECT DB_NAME()");                

            var dbname = Execute<string>(getDatabaseName);

            return dbname;
        }

        public void CreateSchemaHistory()
        {
            const string _SchemaHistory_ = TABLE_SCHEMA_HISTORY;

            var createTable = DatabaseCommand.Text(
                $"CREATE TABLE {_SchemaHistory_} (" +
                "  Seq INT PRIMARY KEY NOT NULL CHECK( Seq > 0 )," +
                "  Name NVARCHAR(256) NOT NULL," +
                "  Hash INT NOT NULL" +
                ")");

            Execute(createTable);
        }

        public void DropSchemaHistory()
        {
            const string _SchemaHistory_ = TABLE_SCHEMA_HISTORY;

            var dropTable = DatabaseCommand.Text
                ($"DROP TABLE {_SchemaHistory_}");

            Execute(dropTable);
        }

        public bool CheckSchemaHistoryExists()
        {
            const string _SchemaHistory_ = TABLE_SCHEMA_HISTORY;

            var createTable = DatabaseCommand.Text
                ("SELECT OBJECT_ID(@schemaHistory)")
                .Parameter("@schemaHistory", _SchemaHistory_);

            var result = Execute<int?>(createTable);

            return (result != null);
        }
        
        public void AddSchemaModification(int seq, string title, int hash)
        {
            const string _SchemaHistory_ = TABLE_SCHEMA_HISTORY;

            if (title == null)
                throw new ArgumentNullException(nameof(title));

            var insertSchemaHistory = DatabaseCommand.Text
                ($"INSERT {_SchemaHistory_}(Seq,Name, Hash) VALUES (@seq,@title, @hash)")
                .Parameter("@seq", seq)
                .Parameter("@title", title)
                .Parameter("@hash", hash);

            Execute(insertSchemaHistory);
        }

        public SchemaChange GetSchemaModification(int seq)
        {
            const string _SchemaHistory_ = TABLE_SCHEMA_HISTORY;

            if (seq <= 0)
                throw new ArgumentOutOfRangeException(nameof(seq));

            var getSchemaHistory = DatabaseCommand.Text
                ($"SELECT Seq, Name, Hash FROM {_SchemaHistory_} WHERE Seq=@seq")
                .Parameter("@seq", seq);

            var results = Execute(getSchemaHistory,
                r =>
                new SchemaChange
                {
                    Seq = (int)r["Seq"],
                    Name = (string)r["Name"],
                    Hash = (int)r["Hash"]
                });

            return results.Single();
        }

        public SchemaChange GetLatestSchemaModification()
        {
            // may return null
            return GetSchemaHistory(1).FirstOrDefault();
        }

        public IEnumerable<SchemaChange> GetSchemaHistory(int rowcount = 1)
        {
            const string _SchemaHistory_ = TABLE_SCHEMA_HISTORY;

            if (rowcount <= 0)
                throw new ArgumentOutOfRangeException(nameof(rowcount));

            var getSchemaHistory = DatabaseCommand.Text
                ($"SELECT TOP(@max) Seq, Name, Hash FROM {_SchemaHistory_} ORDER BY Seq DESC")
                .Parameter("@max", rowcount);

            var results = Execute(getSchemaHistory, 
                r => 
                new SchemaChange {
                    Seq = (int)r["Seq"],
                    Name = (string)r["Name"],
                    Hash = (int)r["Hash"]
                });

            return results;
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
        
        public void ExecuteCommand(string commandText)
        {
            var command = DatabaseCommand.Text(commandText);

            Execute(command);
        }

        void Execute(DatabaseCommand command)
        {
            Execute<object>(command);
        }

        T Execute<T>(DatabaseCommand command)
        {
            T result;

            var connection = GetConnection();

            try
            {
                SqlCommand cmd = command.GetSqlCommand();

                connection.Open();

                cmd.Connection = connection;

                object obj = cmd.ExecuteScalar();

                result = (obj is DBNull) ? default(T) : (T)obj;
            }
            finally
            {
                CloseConnection(connection);
            }

            return result;
        }

        IEnumerable<T> Execute<T>(DatabaseCommand command, Func<IDataRecord,T> getResult)
        {
            List<T> resultSet = new List<T>();

            var connection = GetConnection();

            try
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
            finally
            {
                CloseConnection(connection);
            }

            return resultSet;
        }

        SqlConnection GetConnection()
        {
            if(_currentConnection == null)
            {
                _currentConnection = new SqlConnection(_connectionString);
            }

            return _currentConnection;            
        }

        void CloseConnection(SqlConnection connection)
        {
            if (connection != _currentConnection)
                throw new InvalidOperationException("connection != _currentConnection");

#if DISPOSE_CONNECTION
            _currentConnection.Dispose();
            _currentConnection = null;
#endif         
        }

        public void Dispose()
        {
            if(_currentConnection != null)
            {
                _currentConnection.Dispose();
                _currentConnection = null;
            }
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
