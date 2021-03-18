using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public class View : DataContainer
    {
        public View(string Name, Schema Schema, IEnumerable<Column> ViewColumns)
            : base(Name, Schema, ViewColumns)
        {
        }        
    }
}
