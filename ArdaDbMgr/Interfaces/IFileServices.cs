using System;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Interfaces
{
    public interface IFileServices
    {
        IEnumerable<SqlScript> EnumerateFiles();
    }
}
