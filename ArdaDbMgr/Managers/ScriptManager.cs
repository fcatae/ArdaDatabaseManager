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
        private List<SqlScript> _otherScripts;
        private int _maxIndex = -1;

        public int MaxVersion
        {
            get {
                CheckInitialize();
                return _maxIndex;
            }
        }

        public ScriptManager(IFileServices fileSvcs)
        {
            _fileSvcs = fileSvcs;
        }

        public void Init()
        {
            _index = new SortedDictionary<int, SqlScript>();
            _otherScripts = new List<SqlScript>();

            // enumerate all the scripts
            var scripts = _fileSvcs.EnumerateFiles();

            foreach(var script in scripts)
            {
                int idx = GetIndex(script.Name);

                if (idx <= 0)
                {
                    ReportInvalidScript(script);
                    continue;
                }

                RegisterScript(idx, script);
            }
            
            _isInitialized = true;
        }

        public string ReadScript(int index)
        {
            CheckInitialize();

            if (!_index.ContainsKey(index))
                return null;

            return _index[index].Read();
        }

        public Migration GetScriptMigration(int index)
        {
            CheckInitialize();

            if (!_index.ContainsKey(index))
                throw new ArgumentOutOfRangeException("!_index.ContainsKey(index)");

            SqlScript script = _index[index];

            return new SqlMigration()
            {
                Seq = index,
                Name = script.Name,
                Hash = 0,
                SqlTextCommand = script.Read()
            };
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

        void RegisterScript(int index, SqlScript script)
        {
            if (index > _maxIndex)
                _maxIndex = index;

            _index.Add(index, script);
        }

        void ReportInvalidScript(SqlScript script)
        {
            _otherScripts.Add(script);
        }

        void CheckInitialize()
        {
            if(!_isInitialized)
            {
                Init();
            }
        }        
    }
}
