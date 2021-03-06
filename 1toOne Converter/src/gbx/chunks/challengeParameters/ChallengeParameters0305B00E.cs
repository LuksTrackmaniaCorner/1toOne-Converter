﻿using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class ChallengeParameters0305B00E : Chunk
    {
        public readonly GBXString unknown1;
        public readonly GBXUInt unknown2;
        public readonly GBXUInt unknown3;

        //TODO better names

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
