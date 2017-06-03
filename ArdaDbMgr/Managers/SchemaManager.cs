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
            //throw new InvalidOperationException();
        }

        public int GetLastestVersion()
        {
            // table must exist at this point

            var lastSchemaChange = _databaseSvcs.GetLatestSchemaModification();

            return (lastSchemaChange == null) ? 0 : lastSchemaChange.Seq;
        }
        
        //public Migration GetMigration(int index)
        //{
        //    var lastSchemaChange = _databaseSvcs.GetSchemaChange();

        //    return CreateMigration(lastSchemaChange);
        //}

        //Migration CreateMigration(SchemaChange schema)
        //{
        //    if (schema == null)
        //        return Migration.Zero;

        //    if (schema.Seq <= 0)
        //        throw new ArgumentOutOfRangeException("schema.Seq <= 0");

        //    return new Migration()
        //    {
        //        Seq = schema.Seq,
        //        Name = schema.Name,
        //        Hash = schema.Hash
        //    };
        //}

        public void ApplyMigration(Migration migration)
        {
            if (migration == null)
                throw new ArgumentNullException(nameof(migration));

            migration.Apply(_databaseSvcs);
        }
    }
}
