﻿using System;
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
            Console.WriteLine("Hello DatabaseSchemaManager!");
            
            var test_dbsvcs = new TestDatabaseServices(new SchemaChange[] {
                new SchemaChange { Seq = 1, Name = "001-initial.sql", Hash = 0},
                new SchemaChange { Seq = 2, Name = "002-second.sql", Hash = 0}
                });

            var test_filesvcs = new TestFileServices(
                new string[] {
                    "001-initial.sql",
                    "003-middle.sql",
                    "listing.sql",
                    "004-final.sql",
                    "002-second.sql"
                });

            var mastersvcs = new DatabaseMasterManager("Integrated Security=SSPI");

            mastersvcs.CreateDatabase("DB01");
            mastersvcs.CreateDatabaseFromModel("DB01", "Model");

            mastersvcs.AddDatabaseOwner("arda_dba", "password");
            

            var dbsvcs = new DatabaseServices("Integrated Security=SSPI;Database=DB01");
            var filesvcs = new FileServices("sqlfiles");

            var dbschmgr = new DatabaseSchemaManager(test_dbsvcs, test_filesvcs);
            // specify file pattern:   .UseFiles("sqlfiles", pattern: "(\d+)-.*sql");

            dbschmgr.Init();
            dbschmgr.Upgrade();

            int version = dbschmgr.GetCurrentVersion();

            dbsvcs.Dispose();
        }
        
        void TestDatabaseSchemaManager()
        {

            var test_dbsvcs = new TestDatabaseServices(new SchemaChange[] {
                new SchemaChange { Seq = 1, Name = "001-initial.sql", Hash = 0},
                new SchemaChange { Seq = 2, Name = "002-second.sql", Hash = 0}
                });

            var test_filesvcs = new TestFileServices(
                new string[] {
                    "001-initial.sql",
                    "003-middle.sql",
                    "listing.sql",
                    "004-final.sql",
                    "002-second.sql"
                });

            var dbsvcs = new DatabaseServices("Integrated Security=SSPI;Database=DB01");
            var filesvcs = new FileServices("sqlfiles");

            var dbschmgr = new DatabaseSchemaManager(test_dbsvcs, test_filesvcs);

            dbschmgr.Init();
            dbschmgr.Upgrade();

            int version = dbschmgr.GetCurrentVersion();

            dbsvcs.Dispose();
        }
    }
}