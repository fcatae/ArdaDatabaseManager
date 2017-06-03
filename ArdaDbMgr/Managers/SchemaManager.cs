using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArdaDbMgr.Models;
using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Managers
{
    public class SchemaManager
    {
        private readonly IDatabaseServices _databaseSvcs;

        public SchemaManager(IDatabaseServices databaseSvcs)
        {
            if (databaseSvcs == null)
                throw new ArgumentNullException(nameof(databaseSvcs));

            _databaseSvcs = databaseSvcs;
        }
        
        public void Init()
        {
            string dbname = _databaseSvcs.GetDatabaseName();

            // check it is not master db
            if (dbname == "master")
                throw new InvalidOperationException("dbname == master");

            // check table history exists


            //throw new InvalidOperationException();
        }

        public int GetLastestVersion()
        {
            // table must exist at this point

            var lastSchemaChange = _databaseSvcs.GetLatestSchemaModification();

            // return 0 if history table is empty

            return (lastSchemaChange == null) ? 0 : lastSchemaChange.Seq;
        }

        public Migration GetMigration(int index)
        {
            if (index <= 0)
                throw new ArgumentOutOfRangeException("index <= 0");

            var schema = _databaseSvcs.GetSchemaModification(index);

            if (schema == null)
                throw new InvalidOperationException("schema == null");

            return new Migration()
            {
                Seq = schema.Seq,
                Name = schema.Name,
                Hash = schema.Hash
            };
        }

        public void ApplyMigration(Migration migration)
        {
            if (migration == null)
                throw new ArgumentNullException(nameof(migration));

            migration.Apply(_databaseSvcs);
        }
    }
}
