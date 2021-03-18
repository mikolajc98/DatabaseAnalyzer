using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public class Column
    {
        public string Name { get; private set; }
        public bool Nullable { get; private set; }
        public Models.Type TypeDetails { get; private set; }

        public Column(string name, bool nullable, Models.Type colType)
        {
            this.Name = name;
            this.Nullable = nullable;
            this.TypeDetails = colType;
        }
    }
}
