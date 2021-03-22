using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public abstract class Executable
    {
        public string Name { get; private set; }
        public Schema SchemaDetails { get; private set; }
        public IEnumerable<Parameter> Params { get; private set; }
        public string TypeStr
        {
            get
            {
                return this.GetType().Name;
            }
        }

        internal Executable(string name, Schema schema, IEnumerable<Parameter> parameters)
        {
            this.Name = name;
            this.SchemaDetails = schema;
            this.Params = parameters;
        }
    }
}
