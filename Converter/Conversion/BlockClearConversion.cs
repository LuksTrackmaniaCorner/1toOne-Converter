using Converter.Gbx;
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
    public class BlockClearConversion : Conversion
    {
        public override void Convert(GBXFile file)
        {
            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            blockChunk.Blocks.Clear();
        }
    }
}
