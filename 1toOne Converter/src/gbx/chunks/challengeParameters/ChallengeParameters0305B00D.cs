using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class ChallengeParameters0305B00D : Chunk
    {
        public static readonly string unknownKey = "Unknown";

        public readonly GBXUInt unknown;

        public ChallengeParameters0305B00D(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXUInt(s);
            AddChildDeprevated(unknownKey, unknown);
        }
    }
}
