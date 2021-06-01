﻿using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Chunks.Challenge
{
    public class Challenge03043022 : Chunk
    {
        public static readonly string unknownKey = "Unknown";

        public readonly GBXUInt unknown;

        public Challenge03043022(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXUInt(s);
            AddChildDeprecated(unknownKey, unknown);
        }
    }
}
