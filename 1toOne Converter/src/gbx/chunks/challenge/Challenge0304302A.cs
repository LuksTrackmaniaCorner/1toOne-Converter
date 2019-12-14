using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge0304302A : Chunk
    {
        public static readonly string unknownKey = "Unknown";

        public readonly GBXBool unknown;

        public Challenge0304302A(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXBool(s);
            AddChildDeprecated(unknownKey, unknown);
        }
    }
}
