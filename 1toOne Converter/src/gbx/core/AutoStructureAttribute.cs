using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AutoStructureAttribute : Attribute
    {
        public int Order { get; private set; }

        public AutoStructureAttribute([CallerLineNumber]int order = 0)
        {
            Order = order;
        }
    }
}
