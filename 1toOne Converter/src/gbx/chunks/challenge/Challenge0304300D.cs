﻿using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.chunks
{
    public class Challenge0304300D : Chunk
    {
        public static readonly string metaKey = "Meta";

        private Meta meta;

        public Meta Meta { get => meta; set { meta = value; AddChildNew(value); } }

        public Challenge0304300D(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            Meta = new Meta(s, context);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(metaKey, meta);
            return result;
        }
    }
}
