using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Gbx;
using Converter.Gbx.Chunks.Challenge;
using Converter.Gbx.Core;

namespace Converter.Conversion
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
