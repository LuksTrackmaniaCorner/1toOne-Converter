using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Chunks.ChallengeParameters
{
    public class ChallengeParameters0305B00D : Chunk
    {
        public static readonly string unknownKey = "Unknown";

        public readonly GBXUInt unknown;

        public ChallengeParameters0305B00D(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXUInt(s);
            AddChildDeprecated(unknownKey, unknown);
        }
    }
}
