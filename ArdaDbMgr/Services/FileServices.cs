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
        public IEnumerable<SqlFile> EnumerateFiles(string path)
        {
            var files = from filepath in Directory.EnumerateFiles(path)
                        select new SqlFile(filepath);

            return files;
        }
    }
}
