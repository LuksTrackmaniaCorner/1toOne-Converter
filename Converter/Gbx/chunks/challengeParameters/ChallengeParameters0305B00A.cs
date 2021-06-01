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
            AddChildDeprecated(unknownKey, unknown);

            bronzeTime = new GBXUInt(s);
            AddChildDeprecated(bronzeTimeKey, bronzeTime);

            silverTime = new GBXUInt(s);
            AddChildDeprecated(silverTimeKey, silverTime);

            goldTime = new GBXUInt(s);
            AddChildDeprecated(goldTimeKey, goldTime);

            authorTime = new GBXUInt(s);
            AddChildDeprecated(authorTimeKey, authorTime);

            timeLimit = new GBXUInt(s);
            AddChildDeprecated(timeLimitKey, timeLimit);

            authorScore = new GBXUInt(s);
            AddChildDeprecated(authorScoreKey, authorScore);
        }
    }
}
