using System;
using System.Collections.Generic;
using System.Text;

namespace ArdaDbMgr.Services.Models
{
    public class SqlScript
    {
        protected string _name;
        string _content;

        protected SqlScript()
        {
        }

        public SqlScript(string name, string content)
        {
            _name = name;
            _content = content;
        }

        public string Name { get { return _name; } }

        public virtual string Read()
        {
            return _content;
        }
    }
}
