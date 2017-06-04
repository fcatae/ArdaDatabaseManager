using System;
using System.Collections.Generic;
using System.Text;

namespace ArdaDbMgr
{
    public class DatabaseMasterManager
    {
        private string v;

        public DatabaseMasterManager(string v)
        {
            this.v = v;
        }

        internal void CreateDatabase(string v)
        {
            throw new NotImplementedException();
        }

        internal void CreateDatabaseFromModel(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        internal void AddDatabaseOwner(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
}
