using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;
using ArdaDbMgr.Models;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace ArdaDbMgr.Managers
{
    public class ScriptManager
    {
        private readonly IFileServices _fileSvcs;
        private bool _isInitialized = false;
        private SortedDictionary<int, SqlScript> _index;
        private List<SqlScript> _otherScripts;
        private int _maxIndex = -1;
        private Regex _filePatternRegex;
        private SHA1 _encryptorSha1 = SHA1.Create();

        public int MaxVersion
        {
            get {
                if (_maxIndex <= 0)
                    throw new InvalidOperationException("_maxIndex <= 0");

                return _maxIndex;
            }
        }

        public ScriptManager(IFileServices fileSvcs)
        {
            if (fileSvcs == null)
                throw new ArgumentNullException(nameof(fileSvcs));

            _filePatternRegex = new Regex(@"^(\d\d\d)-");

            _fileSvcs = fileSvcs;
        }

        public ScriptManager(IFileServices fileSvcs, string filePattern)
        {
            if (fileSvcs == null)
                throw new ArgumentNullException(nameof(fileSvcs));

            _filePatternRegex = new Regex(filePattern);

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
        
        public Migration GetScriptMigration(int index)
        {
            CheckInitialize();

            if (!_index.ContainsKey(index))
                throw new ArgumentOutOfRangeException("!_index.ContainsKey(index)");

            SqlScript script = _index[index];
            string text = script.Read();

            return new SqlMigration()
            {
                Seq = index,
                Name = script.Name,
                Hash = CalculateHash(text),
                SqlTextCommand = text
            };
        }

        void CheckInitialize()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("!_isInitialized");
        }

        int GetIndex(string name)
        {
            int index = -1;
            var match = _filePatternRegex.Match(name);

            if (match != null && match.Groups.Count > 1)
            {
                string number = match.Groups[1].Value;
                Int32.TryParse(number, out index);
            }

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
 
        int CalculateHash(string text)
        {            
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = _encryptorSha1.ComputeHash(buf, 0, buf.Length);
            int ret = System.BitConverter.ToInt32(hash, 0);

            return ret;
        }

    }
}
