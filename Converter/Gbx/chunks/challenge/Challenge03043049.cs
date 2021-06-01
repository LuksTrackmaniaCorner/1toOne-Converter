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
    public class Challenge03043049 : Chunk
    {
        public static readonly string unknownKey = "Unknown";

        public Challenge03043049(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            //TODO Figure out what this is.
            var unknown = new Unread(s, 36);
            AddChildDeprecated(unknownKey, unknown);
        }
    }
}
