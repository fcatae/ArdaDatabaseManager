using System;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Services;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Models
{
    public class Migration
    {
        public static Migration Zero = new Migration() { Seq = 0, Name = "--[Migration.Zero]--", Hash = 0 };

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
            databaseService.ExecuteCommand(SqlTextCommand);
            databaseService.AddSchemaModification(this.Seq, this.Name, this.Hash);
        }
    }
}
