using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using ArdaDbMgr.Services.Models;
using ArdaDbMgr.Models;

namespace ArdaDbMgr.Services
{
    public class TestDatabaseServices : IDatabaseServices
    {
        List<SchemaChange> _schemaMods;

        public TestDatabaseServices()
        {
            _schemaMods = new List<SchemaChange>();
        }

        public TestDatabaseServices(IEnumerable<SchemaChange> schemaModList)
        {
            _schemaMods = new List<SchemaChange>(schemaModList);
        }

        public string GetDatabaseName()
        {
            return "TestDatabaseServices";
        }

        public void ExecuteCommand(string commandText)
        {
        }

        public void AddSchemaModification(string title, int hash)
        {
            _schemaMods.Add(new SchemaChange()
            {
                Name = title,
                Hash = hash
            });
        }

        public void AddSchemaModification(int seq, string title, int hash)
        {
            _schemaMods.Add(new SchemaChange()
            {
                Seq = seq,
                Name = title,
                Hash = hash
            });
        }

        public SchemaChange GetLatestSchemaModification()
        {
            int last = _schemaMods.Count - 1;

            if (last < 0)
                return null;

            return _schemaMods[last];
        }

        public SchemaChange GetSchemaModification(int seq)
        {
            var schema = _schemaMods.Find( s => s.Seq == seq );

            if (schema == null)
                throw new InvalidOperationException("_schemaMods.Find( s => s.Seq == seq ) returned null");

            return schema;
        }
    }
}
