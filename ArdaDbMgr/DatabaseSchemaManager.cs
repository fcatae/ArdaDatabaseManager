using System;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Managers;
using ArdaDbMgr.Services;

namespace ArdaDbMgr
{
    class DatabaseSchemaManager
    {
        IDatabaseServices _dbsvcs;
        IFileServices _vfileSvcs;

        public DatabaseSchemaManager(string folder)
        {
        }

        public void Connect(string connectionString)
        {
        }

        public void Init()
        {
            var schemaMgr = new SchemaManager(_dbsvcs);
            var scriptMgr = new ScriptManager(_vfileSvcs);

            schemaMgr.Init();
            scriptMgr.Init();
        }

        public void ValidateLocalFiles()
        {
        }

        public void ValidateRemoteDatabase()
        {
        }

        public void Upgrade()
        {
            var schemaMgr = new SchemaManager(_dbsvcs);
            var scriptMgr = new ScriptManager(_vfileSvcs);

            schemaMgr.Init();
            scriptMgr.Init();

            int version = schemaMgr.GetLastestVersion() + 1;
            int finalVersion = scriptMgr.MaxVersion;

            while (version <= finalVersion)
            {
                var migration = scriptMgr.GetScriptMigration(version);

                schemaMgr.ApplyMigration(migration);

                version++;
            }
        }
    }
}
