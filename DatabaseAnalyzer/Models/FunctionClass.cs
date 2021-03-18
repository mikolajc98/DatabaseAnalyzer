using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public class Function : Executable
    {
        public Function(string name, Schema schema, IEnumerable<Parameter> parameters)
            :base(name, schema, parameters)
        {
        }
    }
}
