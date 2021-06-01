using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Chunks.Challenge
{
    public class Challenge0304302A : Chunk
    {
        public static readonly string unknownKey = "Unknown";

        public readonly GBXBool unknown;

        public Challenge0304302A(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXBool(s);
            AddChildDeprecated(unknownKey, unknown);
        }
    }
}
