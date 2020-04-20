using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class ChallengeParameters0305B004 : Chunk
    {
        public static readonly string bronzeTimeKey = "Bronze Time";
        public static readonly string silverTimeKey = "Silver Time";
        public static readonly string goldTimeKey = "Gold Time";
        public static readonly string authorTimeKey = "Author Time";
        public static readonly string ignoredKey = "Ignored";

        public readonly GBXUInt bronzeTime;
        public readonly GBXUInt silverTime;
        public readonly GBXUInt goldTime;
        public readonly GBXUInt authorTime;
        public readonly GBXUInt ignored;

        public ChallengeParameters0305B004(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            bronzeTime = new GBXUInt(s);
            AddChildDeprecated(bronzeTimeKey, bronzeTime);

            silverTime = new GBXUInt(s);
            AddChildDeprecated(silverTimeKey, silverTime);

            goldTime = new GBXUInt(s);
            AddChildDeprecated(goldTimeKey, goldTime);

            authorTime = new GBXUInt(s);
            AddChildDeprecated(authorTimeKey, authorTime);

            ignored = new GBXUInt(s);
            AddChildDeprecated(ignoredKey, ignored);
        }
    }
}
