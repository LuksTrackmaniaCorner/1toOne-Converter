using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class ChallengeParameters0305B00A : Chunk
    {
        public static readonly string unknownKey = "Unknown";
        public static readonly string bronzeTimeKey = "Bronze Time";
        public static readonly string silverTimeKey = "Silver Time";
        public static readonly string goldTimeKey = "Gold Time";
        public static readonly string authorTimeKey = "Author Time";
        public static readonly string timeLimitKey = "Time Limit";
        public static readonly string authorScoreKey = "Author Score";

        public readonly GBXUInt unknown;
        public readonly GBXUInt bronzeTime;
        public readonly GBXUInt silverTime;
        public readonly GBXUInt goldTime;
        public readonly GBXUInt authorTime;
        public readonly GBXUInt timeLimit;
        public readonly GBXUInt authorScore;

        public ChallengeParameters0305B00A(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXUInt(s);
            AddChildDeprevated(unknownKey, unknown);

            bronzeTime = new GBXUInt(s);
            AddChildDeprevated(bronzeTimeKey, bronzeTime);

            silverTime = new GBXUInt(s);
            AddChildDeprevated(silverTimeKey, silverTime);

            goldTime = new GBXUInt(s);
            AddChildDeprevated(goldTimeKey, goldTime);

            authorTime = new GBXUInt(s);
            AddChildDeprevated(authorTimeKey, authorTime);

            timeLimit = new GBXUInt(s);
            AddChildDeprevated(timeLimitKey, timeLimit);

            authorScore = new GBXUInt(s);
            AddChildDeprevated(authorScoreKey, authorScore);
        }
    }
}
