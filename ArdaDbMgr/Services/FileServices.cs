using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Services
{
    public class FileServices
    {
        private readonly string _path;

        public FileServices(string path)
        {
            _path = path;
        }
        
        public IEnumerable<SqlScript> EnumerateFiles()
        {
            string path = _path;

            string currentDir = Directory.GetCurrentDirectory();

            var files = from filepath in Directory.EnumerateFiles(path)
                        let name = Path.GetFileName(filepath)
                        let fullpath = Path.Combine(currentDir, filepath)
                        select new SqlScriptFile(fullpath);

            return files;
        }

        public string Read(SqlScript file)
        {
            var scriptFile = (SqlScriptFile)file;

            return scriptFile.Read();
        }

        class SqlScriptFile : SqlScript
        {
            private readonly string _path;

            public SqlScriptFile(string path)
            {
                _name = Path.GetFileName(path);
                _path = path;
            }

            public override string Read()
            {
                return File.ReadAllText(_path);
            }
        }
    }
}
