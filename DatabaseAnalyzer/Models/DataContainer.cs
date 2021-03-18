using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public class DataContainer
    {
        public string Name { get; private set; }
        public Schema SchemaDetails { get; private set; }
        public IEnumerable<Column> Columns { get; private set; }
        public string TypeStr
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public DataContainer(string Name, Schema Schema, IEnumerable<Column> Columns)
        {
            this.Name = Name;
            this.SchemaDetails = Schema;
            this.Columns = Columns;
        }
    }
}
