using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Managers
{
    public class ScriptManager
    {
        private readonly FileServices _fileSvcs;
        private bool _isInitialized = false;
        private SortedDictionary<int, SqlScript> _index;

        public ScriptManager(FileServices fileSvcs)
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

        public IEnumerable<SqlScript> GetPendingScripts(int firstIndex=1)
        {
            if (firstIndex <= 0)
                throw new ArgumentOutOfRangeException("firstIndex must be equal or greater than 1");

            CheckInitialize();

            var scripts = from entry in _index
                          where entry.Key >= firstIndex
                          select entry.Value;

            return scripts;
        }

        public IEnumerable<SchemaModification> GetPendingChanges(int firstIndex = 1)
        {
            if (firstIndex <= 0)
                throw new ArgumentOutOfRangeException("firstIndex must be equal or greater than 1");

            CheckInitialize();

            var scripts = from entry in _index
                          where entry.Key >= firstIndex
                          select new SchemaModification()
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
