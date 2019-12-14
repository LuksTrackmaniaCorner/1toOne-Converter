using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.conversion
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
