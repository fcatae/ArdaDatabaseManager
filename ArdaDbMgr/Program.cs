using System;
using System.Diagnostics;
using System.Linq;
using ArdaDbMgr.Managers;
using ArdaDbMgr.Models;
using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            // Enumerate files
            // Connect to database
            // Check database is created
            // Create database if needed
            // Check schema history table
            // Get the latest update
            // Get the pending schema modifications

            var dbsvcs = new VirtualDatabaseServices(new SchemaChange[] {
                new SchemaChange { Seq = 1, Name = "001-initial.sql", Hash = 0},
                new SchemaChange { Seq = 2, Name = "002-second.sql", Hash = 0}
                });

            var vfileSvcs = new VirtualFileServices(
                new string[] {
                    "001-initial.sql",
                    "003-middle.sql",
                    "listing.sql",
                    "004-final.sql",
                    "002-second.sql"
                });

            var dbschmgr = new DatabaseSchemaManager(dbsvcs, vfileSvcs);

            dbschmgr.Init();
            dbschmgr.Upgrade();

            int version = dbschmgr.GetCurrentVersion();
        }
        
    }
}