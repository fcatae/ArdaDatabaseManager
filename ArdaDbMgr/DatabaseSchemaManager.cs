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
        IFileServices _fileSvcs;
        SchemaManager _schemaMgr;
        ScriptManager _scriptMgr;

        public DatabaseSchemaManager(IDatabaseServices dbsvcs, IFileServices fileSvcs)
        {
            _dbsvcs = dbsvcs;
            _fileSvcs = fileSvcs;
        }

        public DatabaseSchemaManager(string folder)
        {
        }

        public void Connect(string connectionString)
        {
        }

        public void Init()
        {
            _schemaMgr = new SchemaManager(_dbsvcs);
            _scriptMgr = new ScriptManager(_fileSvcs);

            _schemaMgr.Init();
            _scriptMgr.Init();
        }

        public void ValidateLocalFiles()
        {
        }

        public void ValidateRemoteDatabase()
        {
        }

        public void Upgrade()
        {
            var schemaMgr = _schemaMgr;
            var scriptMgr = _scriptMgr;

            int version = schemaMgr.GetLastestVersion() + 1;
            int finalVersion = scriptMgr.MaxVersion;

            while (version <= finalVersion)
            {
                var migration = scriptMgr.GetScriptMigration(version);

                schemaMgr.ApplyMigration(migration);

                version++;
            }
        }

        public int GetCurrentVersion()
        {
            return _schemaMgr.GetLastestVersion();
        }
    }
}
