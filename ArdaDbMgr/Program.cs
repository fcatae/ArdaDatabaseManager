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

            var dbsvcsInit = new DatabaseServices("Integrated Security=SSPI");
            if (!dbsvcsInit.VerifyDatabase("DB003Managers"))
            {
                dbsvcsInit.CreateDatabase("DB003Managers");
            }
            var dbsvcs = new DatabaseServices("Integrated Security=SSPI;Database=DB003Managers");

            if (dbsvcs.CheckSchemaHistoryExists())
                dbsvcs.DropSchemaHistory();

            dbsvcs.CreateSchemaHistory();
            
            dbsvcs.AddSchemaModification("Initial", 10);
            dbsvcs.AddSchemaModification("Second", 20);

            var dbmgr = new SchemaManager(dbsvcs);
            var lastmigration = dbmgr.GetLastChange();

            var vfileSvcs = new VirtualFileServices(
                new string[] {
                    "001-initial.sql",
                    "003-middle.sql",
                    "listing.sql",
                    "004-final.sql",
                    "002-second.sql"
                });

            var scriptMgr = new ScriptManager(vfileSvcs);

            var list = scriptMgr.GetPendingChanges(lastmigration).ToArray();

            foreach(var s in list)
            {
                string val = scriptMgr.ReadText(s);
            }

        }
        
    }
}