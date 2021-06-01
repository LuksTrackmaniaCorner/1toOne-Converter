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
    public class ChallengeParameters0305B00E : Chunk
    {
        //TODO better names
        public readonly GBXString unknown1;
        public readonly GBXUInt unknown2;
        public readonly GBXUInt unknown3;

        public ChallengeParameters0305B00E(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown1 = new GBXString(s);
            AddChildDeprecated("Unknown 1", unknown1);

            unknown2 = new GBXUInt(s);
            AddChildDeprecated("Unknown 2", unknown2);

            unknown3 = new GBXUInt(s);
            AddChildDeprecated("Unknown 3", unknown3);
        }
    }
}
