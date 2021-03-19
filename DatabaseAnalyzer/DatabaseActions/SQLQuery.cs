using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.DatabaseActions
{
    public abstract class SQLQuery<T>
    {
        private readonly string SQLScript;

        public string GetScript()
        {
            return SQLScript;
        }
        abstract public IEnumerable<T> ExecuteAndReturn();
        private DatabaseAnalyzer.Main.Database.ObjectCreator Creator { get; set; } = null;
    }
}
