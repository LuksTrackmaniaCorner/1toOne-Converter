using _1toOne_Converter.src.gbx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.conversion
{
    [XmlRoot]
    [XmlInclude(typeof(BlockAddConversion))]
    [XmlInclude(typeof(BlockClearConversion))]
    [XmlInclude(typeof(BlockToItemConversion))]
    [XmlInclude(typeof(CarConversion))]
    [XmlInclude(typeof(EnviConversion))]
    [XmlInclude(typeof(GroundItemAddConversion))]
    [XmlInclude(typeof(ItemAddConversion))]
    [XmlInclude(typeof(TitleConversion))]
    public abstract class Conversion
    {
        public abstract void Convert(GBXFile file);
    }
}
