﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public class Type
    {
        public string Name { get; private set; }
        public string Precision { get; private set; }
        public string DefaultValue { get; private set; }

        public Type(string Name, string Precision, string DefaultValue)
        {
            this.Name = Name;
            this.Precision = Precision;
            this.DefaultValue = DefaultValue;
        }
    }
}
