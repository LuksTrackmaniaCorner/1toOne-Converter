using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class ChallengeCommunity : Chunk
    {
        public static readonly string xmlKey = "XML";

        public GBXString xml;

        public ChallengeCommunity(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            xml = new GBXString(s);
            AddChildDeprevated(xmlKey, xml);
        }
    }
}
