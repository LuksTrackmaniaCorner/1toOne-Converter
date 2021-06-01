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
    public class Challenge03043018 : Chunk
    {
        public static readonly string unknownKey = "Unknown";
        public static readonly string numLapsKey = "Number of Laps";

        public readonly GBXBool unknown;
        public readonly GBXUInt numLaps;

        public Challenge03043018(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXBool(s);
            AddChildDeprecated(unknownKey, unknown);

            numLaps = new GBXUInt(s);
            AddChildDeprecated(numLapsKey, numLaps);
        }
    }
}
