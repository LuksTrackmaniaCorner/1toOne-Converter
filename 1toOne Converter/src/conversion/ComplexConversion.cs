﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using _1toOne_Converter.src.gbx;

namespace _1toOne_Converter.src.conversion
{
    public class ComplexConversion : Conversion
    {
        public List<Conversion> Conversions;

        internal override void Initialize()
        {
            foreach(var conversion in Conversions)
            {
                conversion.Initialize();
            }
        }

        public override void Convert(GBXFile file)
        {
            foreach(var conversion in Conversions)
            {
                conversion.Convert(file);
            }
        }
    }
}
