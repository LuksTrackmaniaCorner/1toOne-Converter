using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class ChallengeParameters0305B008 : Chunk
    {
        public static readonly string timeLimitKey = "Time Limit";
        public static readonly string authorScoreKey = "Author Score";

        public readonly GBXUInt timeLimit;
        public readonly GBXUInt authorScore;

        public ChallengeParameters0305B008(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            timeLimit = new GBXUInt(s);
            AddChildDeprecated(timeLimitKey, timeLimit);

            authorScore = new GBXUInt(s);
            AddChildDeprecated(authorScoreKey, authorScore);
        }
    }
}
