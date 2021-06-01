using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Converter.Gbx;
using Converter.Gbx.Chunks.Challenge;
using Converter.Gbx.Core;

namespace Converter.Conversion
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
