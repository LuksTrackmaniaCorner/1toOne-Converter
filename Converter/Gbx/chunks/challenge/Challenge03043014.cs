using Converter.Gbx.core;
using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.chunks
{
    public class Challenge03043014 : Chunk
    {
        public static readonly string unknownKey = "Unknown";
        public static readonly string passwordXorOldKey = "Password Xor Old";

        public readonly GBXUInt unknown;
        public readonly GBXString passwordXorOld;

        public Challenge03043014(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXUInt(s);
            AddChildDeprecated(unknownKey, unknown);

            passwordXorOld = new GBXString(s);
            AddChildDeprecated(passwordXorOldKey, passwordXorOld);
        }
    }
}
