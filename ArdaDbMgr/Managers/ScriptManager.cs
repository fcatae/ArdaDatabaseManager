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
        private SortedDictionary<int, SqlScript> _index = new SortedDictionary<int, SqlScript>();
        private List<SqlScript> _otherScripts = new List<SqlScript>();
        private int _maxIndex = -1;

        public int MaxIndex
            {
            get { return _maxIndex;  }}

        public ScriptManager(IFileServices fileSvcs)
        {
            _fileSvcs = fileSvcs;
        }

        public void Init()
        {
            // enumerate all the scripts
            var scripts = _fileSvcs.EnumerateFiles();

            foreach(var script in scripts)
            {
                int idx = GetIndex(script.Name);

                SetMaxIndex(idx);
                RegisterScript(idx, script);
            }
            
            _isInitialized = true;
        }

        void SetMaxIndex(int index)
        {
            if (index > _maxIndex)
                _maxIndex = index;
        }

        void RegisterScript(int index, SqlScript script)
        {
            if (index > 0)
            {
                // valid script
                _index.Add(index, script);
            }
            else
            {
                _otherScripts.Add(script);
            }
        }

        void CheckInitialize()
        {
            if(!_isInitialized)
            {
                Init();
            }
        }
        
        public string ReadScript(int index)
        {
            if (!_index.ContainsKey(index))
                return null;

            return _index[index].Read();
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
        
    }
}
