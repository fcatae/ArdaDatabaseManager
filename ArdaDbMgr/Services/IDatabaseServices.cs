using ArdaDbMgr.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArdaDbMgr.Services
{
    public interface IDatabaseServices
    {
        SchemaModification GetLatestSchemaModification();

        void ExecuteCommand(string commandText);
    }
}
