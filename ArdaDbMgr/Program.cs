using System;
using System.Diagnostics;
using System.Linq;
using ArdaDbMgr.Services;

namespace ArdaDbMgr
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var dbmgr = new DatabaseSchemaManager("sqlfiles");

            dbmgr.Connect("connectionString");

            dbmgr.StartUpgrade();

            // Enumerate files
            // Connect to database
            // Check database is created
            // Create database if needed
            // Check schema history table
            
            var dbsvcsInit = new DatabaseServices("Integrated Security=SSPI");
            if(!dbsvcsInit.VerifyDatabase("DB002SchemaHistory"))
            {
                dbsvcsInit.CreateDatabase("DB002SchemaHistory");
            }

            var dbsvcs = new DatabaseServices("Integrated Security=SSPI;Database=DB002SchemaHistory");
            dbsvcs.CreateSchemaHistory();
        }
    }
}