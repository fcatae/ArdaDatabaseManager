using System;
using System.Linq;
using Xunit;

using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;
using ArdaDbMgr.Managers;

namespace ArdaDbMgrTest
{
    public class SchemaManagerTest
    {
        [Fact]
        public void ApplyMigrations()
        {
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

            var schemaMgr = new SchemaManager(dbsvcs);
            var scriptMgr = new ScriptManager(vfileSvcs);

            schemaMgr.Init();
            scriptMgr.Init();

            int version = schemaMgr.GetLastestVersion() + 1;
            int finalVersion = scriptMgr.MaxVersion;

            var info2 = schemaMgr.GetMigration(2);

            Assert.NotNull(info2);
            Assert.Equal(2, info2.Seq);
            Assert.Equal("002-second.sql", info2.Name);

            Assert.Throws(typeof(InvalidOperationException), () => {
                info2 = schemaMgr.GetMigration(3);
            });            

            while (version <= finalVersion)
            {
                var migration = scriptMgr.GetScriptMigration(version);

                schemaMgr.ApplyMigration(migration);

                version++;
            }

            var info4 = schemaMgr.GetMigration(4);

            Assert.NotNull(info4);
            Assert.Equal(4, info4.Seq);
        }
    }
}
