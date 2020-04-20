using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.core.chunks
{
    public class ChallengeVersion :Chunk
    {
        public static readonly string versionKey = "Version";

        public readonly GBXUInt version;
        public ChallengeVersion(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            version = new GBXUInt(s);
            AddChildDeprecated(versionKey, version);
        }
    }
}
