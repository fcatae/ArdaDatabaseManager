using System;
using System.Diagnostics;
using System.Linq;
using ArdaDbMgr.Managers;
using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var dbschmgr = new DatabaseSchemaManager("sqlfiles");

            dbschmgr.Connect("connectionString");

            dbschmgr.StartUpgrade();

            // Enumerate files
            // Connect to database
            // Check database is created
            // Create database if needed
            // Check schema history table
            // Get the latest update
            // Get the pending schema modifications
            
            var dbsvcs = new VirtualDatabaseServices(new SchemaModification[] {
                new SchemaModification { Seq = 1, Name = "001-initial.sql", Hash = 0},
                new SchemaModification { Seq = 2, Name = "002-second.sql", Hash = 0}
                });

            var vfileSvcs = new VirtualFileServices(
                new string[] {
                    "001-initial.sql",
                    "003-middle.sql",
                    "listing.sql",
                    "004-final.sql",
                    "002-second.sql"
                });

            var dbmgr = new SchemaManager(dbsvcs);
            var scriptMgr = new ScriptManager(vfileSvcs);

            scriptMgr.Init();

            int lastVersion = dbmgr.GetLastestVersion().Seq;
            int finalVersion = scriptMgr.MaxIndex;

            for(int version=lastVersion+1; version<finalVersion; version++)
            {
                string text = scriptMgr.ReadScript(version);

                if (text == null)
                    throw new InvalidOperationException("skipped index");
            }
        }
        
    }
}