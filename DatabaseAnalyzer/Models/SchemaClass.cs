using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public class Schema
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public Schema(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
