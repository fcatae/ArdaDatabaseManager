using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArdaDbMgr.Models;
using ArdaDbMgr.Services;

namespace ArdaDbMgr.Managers
{
    public class SchemaManager
    {
        private readonly DatabaseServices _databaseSvcs;

        public SchemaManager(DatabaseServices databaseSvcs)
        {
            _databaseSvcs = databaseSvcs;
        }

        public Migration GetLastChange()
        {
            // current database exists?

            // return null;???

            //
            var lastSchemaChange = _databaseSvcs.GetSchemaHistory().FirstOrDefault();

            return (lastSchemaChange != null) ? new Migration(lastSchemaChange) : Migration.Zero;
        }

        public void Apply(Migration migration)
        {

        }

        // new interface
        public Migration GetLatestVersion()
        {
            throw new InvalidOperationException();
        }

        public void UpgradeVersion(Migration migration)
        {
            throw new InvalidOperationException();
        }

        void BeginTransaction()
        {
            throw new InvalidOperationException();
        }

        void ModifySchema(Migration migration)
        {
            throw new InvalidOperationException();
        }

        void CommitTransaction()
        {
            throw new InvalidOperationException();
        }
    }
}
