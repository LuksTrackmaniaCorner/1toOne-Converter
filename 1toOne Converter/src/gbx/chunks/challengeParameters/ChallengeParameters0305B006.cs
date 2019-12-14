using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class ChallengeParameters0305B006 : Chunk
    {
        public static readonly string countKey = "Count";

        public readonly GBXUInt count;

        public ChallengeParameters0305B006(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            count = new GBXUInt(s);
            Trace.Assert(count.Value== 0, "Unsupported count");
            AddChildDeprecated(countKey, count);
        }
    }
}
