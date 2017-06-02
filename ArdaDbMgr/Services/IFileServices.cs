using System;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Services
{
    public interface IFileServices
    {
        IEnumerable<SqlScript> EnumerateFiles();
    }
}
