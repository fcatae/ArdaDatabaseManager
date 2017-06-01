using System;
using System.Linq;
using Xunit;

using ArdaDbMgr.Services;

namespace ArdaDbMgrTest
{
    public class DatabaseServicesTest
    {
        [Fact]
        public void CreateAndDeleteDatabases()
        {
            var dbsvcs = new DatabaseServices("Integrated Security=SSPI");

            // setup
            if(dbsvcs.VerifyDatabase("DB001"))
                dbsvcs.DropDatabase("DB001");

            // test
            bool dbExistsBefore = dbsvcs.VerifyDatabase("DB001");

            dbsvcs.CreateDatabase("DB001");

            bool dbExistsAfter = dbsvcs.VerifyDatabase("DB001");

            dbsvcs.DropDatabase("DB001");

            bool dbExistsAfterDelete = dbsvcs.VerifyDatabase("DB001");

            Assert.False(dbExistsBefore);
            Assert.True(dbExistsAfter);
            Assert.False(dbExistsAfterDelete);
        }

        [Fact]
        public void CreateSchemaHistory()
        {
            // setup
            var dbsvcsInit = new DatabaseServices("Integrated Security=SSPI");
            if (!dbsvcsInit.VerifyDatabase("DB002SchemaHistory"))
            {
                dbsvcsInit.CreateDatabase("DB002SchemaHistory");
            }

            // test
            var dbsvcs = new DatabaseServices("Integrated Security=SSPI;Database=DB002SchemaHistory");

            bool beforeSchemaHistory = dbsvcs.CheckSchemaHistoryExists();

            dbsvcs.CreateSchemaHistory();
            bool afterSchemaHistory = dbsvcs.CheckSchemaHistoryExists();

            dbsvcs.DropSchemaHistory();
            bool afterSchemaHistoryDelete = dbsvcs.CheckSchemaHistoryExists();

            Assert.False(beforeSchemaHistory);
            Assert.True(afterSchemaHistory);
            Assert.False(afterSchemaHistoryDelete);
        }
    }
}
