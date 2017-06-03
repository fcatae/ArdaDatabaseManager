using ArdaDbMgr.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArdaDbMgr.Services
{
    public interface IDatabaseServices
    {
        SchemaChange GetLatestSchemaModification();
        void AddSchemaModification(int seq, string title, int hash);
        SchemaChange GetSchemaModification(int seq);
        void ExecuteCommand(string commandText);
    }
}
