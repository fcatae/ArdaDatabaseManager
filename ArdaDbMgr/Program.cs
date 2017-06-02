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
            
            var vfileSvcs = new VirtualFileServices(
                new string[] {
                    "001-initial.sql",
                    "003-middle.sql",
                    "listing.sql",
                    "004-final.sql",
                    "002-second.sql"
                });

            var scriptMgr = new ScriptManager(vfileSvcs);

            var list = scriptMgr.GetPendingChanges(2).ToArray();

        }
        
    }
}