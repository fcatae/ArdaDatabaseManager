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

            var dbsvcs = new DatabaseServices("Integrated Security=SSPI;Database=DB002SchemaHistory");

            if (dbsvcs.CheckSchemaHistoryExists())
                dbsvcs.DropSchemaHistory();

            // test
            bool beforeSchemaHistory = dbsvcs.CheckSchemaHistoryExists();

            dbsvcs.CreateSchemaHistory();
            bool afterSchemaHistory = dbsvcs.CheckSchemaHistoryExists();

            // add & get
            dbsvcs.AddSchemaModification("Initial", 10);
            dbsvcs.AddSchemaModification("Second", 20);
            dbsvcs.AddSchemaModification("Third", 30);

            var modifications = dbsvcs.GetSchemaHistory(5).ToArray();
            Assert.Equal("Third", modifications[0].Name );
            Assert.Equal(10, modifications[2].Hash );

            var lastModification = dbsvcs.GetSchemaHistory().ToArray();
            Assert.Equal(30, lastModification[0].Hash);

            // drop
            dbsvcs.DropSchemaHistory();
            bool afterSchemaHistoryDelete = dbsvcs.CheckSchemaHistoryExists();

            Assert.False(beforeSchemaHistory);
            Assert.True(afterSchemaHistory);
            Assert.False(afterSchemaHistoryDelete);
        }
    }
}
