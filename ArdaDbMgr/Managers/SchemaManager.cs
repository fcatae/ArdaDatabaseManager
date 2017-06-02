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
        private readonly IDatabaseServices _databaseSvcs;

        public SchemaManager(IDatabaseServices databaseSvcs)
        {
            _databaseSvcs = databaseSvcs;
        }
        
        public Migration GetLastestVersion()
        {
            // current database exists?

            // return null;???

            //
            var lastSchemaChange = _databaseSvcs.GetLatestSchemaModification();

            return (lastSchemaChange != null) ? new Migration(lastSchemaChange) : Migration.Zero;
        }
        

        public void UpgradeVersion(Migration migration)
        {
            throw new InvalidOperationException();
        }      
    }
}
