using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Services
{
    public class TestFileServices : IFileServices
    {
        SqlScript[] _scriptList; 

        public TestFileServices(IEnumerable<string> filenames)
        {
            int identity = 1;
            var sqlscripts = from filename in filenames
                             select new SqlScript(filename, 
                                $"CREATE TABLE tb{identity++}(id INT) -- content file = {filename}");

            _scriptList = sqlscripts.ToArray();
        }

        public IEnumerable<SqlScript> EnumerateFiles()
        {
            return _scriptList.ToArray();
        }
    }
}
