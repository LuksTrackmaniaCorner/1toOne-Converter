using _1toOne_Converter.src.gbx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.conversion
{
    [XmlRoot]
    [XmlInclude(typeof(BlockAddConversion))]
    [XmlInclude(typeof(BlockClearConversion))]
    [XmlInclude(typeof(BlockToBlockConversion))]
    [XmlInclude(typeof(BlockToItemConversion))]
    [XmlInclude(typeof(CarConversion))]
    [XmlInclude(typeof(EnviConversion))]
    [XmlInclude(typeof(GridDefineConversion))]
    [XmlInclude(typeof(GroundItemAddConversion))]
    [XmlInclude(typeof(ItemAddConversion))]
    [XmlInclude(typeof(ItemClipAddConversion))]
    [XmlInclude(typeof(MultiBlockAddConversion))]
    [XmlInclude(typeof(PylonAddConversion))]
    [XmlInclude(typeof(TerrainMappingConversion))]
    [XmlInclude(typeof(TitleConversion))]
    public abstract class Conversion
    {
        /// <summary>
        /// This method will be called after the Conversion has been deserialized after the xml File
        /// </summary>
        internal virtual void Initialize()
        {

        }

        public abstract void Convert(GBXFile file);

        public static T LoadConversion<T>(string xmlFile) where T:Conversion
        {
            T conversion;
            var xmls = new XmlSerializer(typeof(T));
            using var fs = new FileStream(xmlFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            conversion = (T)xmls.Deserialize(fs);
            conversion.Initialize();

            return conversion;
        }
    }
}
