using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;

namespace _1toOne_Converter.src.conversion
{
    [Obsolete("Should make use of MinimalItem", false)]
    public class ItemAddConversion : Conversion
    {
        public List<Node> ExtraItems;

        public override void Convert(GBXFile file)
        {
            var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);
            itemChunk.Items.AddAll(ExtraItems);
        }
    }
}
