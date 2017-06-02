using System;
using System.Collections.Generic;
using System.Text;
using ArdaDbMgr.Services.Models;

namespace ArdaDbMgr.Models
{
    public class Migration
    {
        public static Migration Zero = new Migration() { Seq = 0, Name = "--[Migration.Zero]--", Hash = 0 };

        public int Seq;
        public string Name;
        public int Hash;

        public Migration()
        {
        }

        public Migration(SchemaModification schema)
        {
            this.Seq = schema.Seq;
            this.Name = schema.Name;
            this.Hash = schema.Hash;
        }
    }
}
