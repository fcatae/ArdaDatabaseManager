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

        public DatabaseSchemaManager Folder(string folder, string pattern=null)
        {
            // define the base folder with scripts
            throw new InvalidOperationException();
        }
        
        public DatabaseSchemaManager Connect(string connectionString, string scripthistory="_ScriptHistory_")
        {
            // define the target database
            throw new InvalidOperationException();
        }

        // removed:
        // - master responsibilities
        // - database cloning
        // - data seeding
        // - final db script
        // - bcp 

        // create user?
        // create schema?
        // create new connection string?
        // initial seed?

        public void Run()
        {
            // test everything is fine
            // no version conflicts
            // data is ok
            // undo/redo
            // redo/redo
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
