﻿using Converter.Gbx;
using Converter.Gbx.chunks;
using Converter.Gbx.core;
using Converter.Gbx.core.chunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Converion
{
    public class CarConversion : Conversion
    {
        public Meta CarMeta;

        public override void Convert(GBXFile file)
        {
            var chunk0304300D = (Challenge0304300D)file.GetChunk(Chunk.challenge0304300DKey);

            chunk0304300D.Meta = (Meta) CarMeta.DeepClone();
        }
    }
}