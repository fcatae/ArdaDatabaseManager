using System;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Models
{
    public class Migration
    {
        public int Seq;
        public string Name;
        public int Hash;
        
        public virtual void Apply(IDatabaseServices databaseService)
        {
            databaseService.AddSchemaModification(this.Seq, this.Name, this.Hash);
        }
    }

    public class SqlMigration : Migration
    {
        public string SqlTextCommand;

        public override void Apply(IDatabaseServices databaseService)
        {
            // no transaction -- if the commands fail in the middle
            databaseService.ExecuteCommand(SqlTextCommand);
            databaseService.AddSchemaModification(this.Seq, this.Name, this.Hash);
        }
    }
}
