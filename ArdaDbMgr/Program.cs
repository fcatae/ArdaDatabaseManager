using System;
using System.Diagnostics;
using System.Linq;
using ArdaDbMgr.Managers;
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
            // Get the latest update
            // Get the pending schema modifications

            var fileSvcs = new FileServices("sqlfiles");
            var scriptMgr = new ScriptManager(fileSvcs);

            var list = scriptMgr.GetPendingChanges(2).ToArray();

        }
        
    }
}