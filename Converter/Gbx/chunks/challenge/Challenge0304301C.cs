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
    public class Challenge0304301C : Chunk
    {
        public static readonly string playModeKey = "Play Mode";

        public readonly GBXUInt playMode;

        public Challenge0304301C(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            playMode = new GBXUInt(s);
            AddChildDeprecated(playModeKey, playMode);
        }
    }
}
