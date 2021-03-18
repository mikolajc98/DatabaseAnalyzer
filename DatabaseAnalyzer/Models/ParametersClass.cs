using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public class Parameter : Column
    {
        public bool IsTableParam { get; private set; }

        public Parameter(string name, bool nullable, Models.Type paramType, bool isTable)
            :base(name, nullable, paramType)
        {
            this.IsTableParam = isTable;
        }

    }
}
