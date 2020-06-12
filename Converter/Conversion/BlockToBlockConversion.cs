using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Converter.Gbx;
using Converter.Gbx.chunks;
using Converter.Gbx.core;
using Converter.Gbx.core.primitives;

namespace Converter.Converion
{
    public class BlockToBlockConversion : Conversion
    {
        [XmlElement(ElementName = "BlockToBlock")]
        public BlockToBlock[] BlockToBlocks;

        private readonly Dictionary<string, BlockToBlock> _blockDict;

        public BlockToBlockConversion()
        {
            _blockDict = new Dictionary<string, BlockToBlock>();
        }

        internal override void Initialize()
        {
            foreach(var blockToBlock in BlockToBlocks)
            {
                _blockDict.Add(blockToBlock.OldName, blockToBlock);
            }
        }

        public override void Convert(GBXFile file)
        {
            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);

            foreach(var block in blockChunk.Blocks)
            {
                var blockName = block.BlockName.Content;

                if (_blockDict.ContainsKey(blockName))
                {
                    var blockToBlock = _blockDict[blockName];
                    block.BlockName= new GBXLBS(blockToBlock.NewName);
                    block.Coords.Y += blockToBlock.YOffset;
                }
            }
        }
    }

    public class BlockToBlock
    {
        [XmlAttribute]
        public string OldName;
        [XmlAttribute]
        public string NewName;
        [XmlAttribute]
        public byte YOffset;
    }
}
