using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;

namespace _1toOne_Converter.src.conversion
{
    public class BlockAddConversion : Conversion
    {
        public List<Block> ExtraBlocks;

        public override void Convert(GBXFile file)
        {
            var blockChunk = (Challenge0304301F) file.GetChunk(Chunk.challenge0304301FKey);

            blockChunk.Version.Value = 6; //Adding Support for Custom Blocks

            foreach(var block in ExtraBlocks)
            {
                blockChunk.Blocks.Add((Block) block.DeepClone());
            }
        }
    }
}
