using Converter.Gbx;
using Converter.Gbx.Chunks.Challenge;
using Converter.Gbx.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Conversion
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
