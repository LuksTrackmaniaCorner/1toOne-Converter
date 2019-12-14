using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1toOne_Converter.src.gbx;

namespace _1toOne_Converter.src.conversion
{
    public class ComplexConversion : Conversion
    {
        public List<Conversion> Conversions;

        public override void Convert(GBXFile file)
        {
            foreach(var conversion in Conversions)
            {
                conversion.Convert(file);
            }
        }
    }
}
