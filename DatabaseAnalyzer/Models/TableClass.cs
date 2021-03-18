using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public class Table : DataContainer
    {
        public Table(string Name, Schema Schema, IEnumerable<Column> TableColumns)
            : base(Name, Schema, TableColumns)
        {
        }

    }
}
