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

            bool dbExistsBefore = dbsvcs.VerifyDatabase("DB001");

            dbsvcs.CreateDatabase("DB001");

            bool dbExistsAfter = dbsvcs.VerifyDatabase("DB001");

            dbsvcs.DropDatabase("DB001");

            bool dbExistsAfterDelete = dbsvcs.VerifyDatabase("DB001");

            Assert.False(dbExistsBefore);
            Assert.True(dbExistsAfter);
            Assert.False(dbExistsAfterDelete);
        }
    }
}
