using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;
using ArdaDbMgr.Models;

namespace ArdaDbMgr.Managers
{
    public class ScriptManager
    {
        private readonly IFileServices _fileSvcs;
        private bool _isInitialized = false;
        private SortedDictionary<int, SqlScript> _index;

        public ScriptManager(IFileServices fileSvcs)
        {
            _fileSvcs = fileSvcs;
        }

        void Init()
        {
            // enumerate all the scripts
            var scripts = _fileSvcs.EnumerateFiles();

            // create the index
            var index = CreateIndex(scripts);

            _isInitialized = true;
            _index = index;
        }

        void CheckInitialize()
        {
            if(!_isInitialized)
            {
                Init();
            }
        }

        public string ReadText(Migration migration)
        {
            return _index[migration.Seq].Read();
        }

        public IEnumerable<Migration> GetPendingChanges(Migration migration)
        {
            return GetPendingChanges(migration.Seq);
        }

        public IEnumerable<Migration> GetPendingChanges(int lastIndex = 0)
        {
            if (lastIndex < 0)
                throw new ArgumentOutOfRangeException("firstIndex must be equal or greater than 0");

            CheckInitialize();

            var scripts = from entry in _index
                          where entry.Key > lastIndex
                          select new Migration()
                          {
                              Seq = entry.Key,
                              Name = entry.Value.Name,
                              Hash = 0
                          };

            return scripts;
        }

        int GetIndex(string name)
        {
            if (name == null || name.Length <= 4)
                return -1;

            string prefix = name.Substring(0, 3);

            int index = -1;

            Int32.TryParse(prefix, out index);

            return index;
        }

        SortedDictionary<int,SqlScript> CreateIndex(IEnumerable<SqlScript> scripts)
        {
            var index = new SortedDictionary<int, SqlScript>();
            List<SqlScript> invalidFiles = new List<SqlScript>();

            foreach(var script in scripts)
            {
                string name = script.Name;
                int idx = GetIndex(name);

                if(idx > 0)
                {
                    index.Add(idx, script);
                }

                if(idx == 0 || idx < 0)
                {
                    invalidFiles.Add(script);
                }
            }

            return index;
        }
    }
}
