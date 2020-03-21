using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.conversion
{
    public class BlockToItemConversion : Conversion
    {
        public GBXLBS Collection;
        public GBXLBS DefaultAuthor;

        public FlagName SecondaryTerrainFlag;
        [XmlElement(IsNullable = false, ElementName = "BlockIgnoreFlag")]
        public FlagName[] BlockIgnoreFlags;

        public List<BlockData> Blocks;
        private readonly Dictionary<string, BlockData> _blockNameDict;

        public FlagName ItemCountStatistic;

        public BlockToItemConversion()
        {
            _blockNameDict = new Dictionary<string, BlockData>();
        }

        internal override void Initialize()
        {
            foreach (var block in Blocks)
            {
                block.Initialize();
                //TODO Add method getnames to blockData
                if(block.BlockName != null)
                    _blockNameDict.Add(block.BlockName, block);

                foreach (var altName in block.AltNames)
                    _blockNameDict.Add(altName.BlockName, block);
            }
        }

        public override void Convert(GBXFile file)
        {
            int itemCount = 0;

            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);
            var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);

            if (itemChunk == null)
            {
                itemChunk = new Challenge03043040(false);
                file.AddBodyChunk(Chunk.challenge03043040Key, itemChunk);
            }

            foreach (var block in blockChunk.Blocks)
            {
                /*
                if(block.BlockName.Content == "BayBuilding2Pillar" && ((block.Flags.Value & 0x1000) != 0 ))
                {
                    uint a = block.Flags.Value & 0x3F;
                    uint b = (block.Flags.Value >> 6) & 0x3F;
                    Console.WriteLine($"Variante: {a} Random: {b}");
                }
                */

                if (!_blockNameDict.ContainsKey(block.BlockName.Content))
                    continue; //block cannot be converted;

                var blockData = _blockNameDict[block.BlockName.Content];
                var adjustedCoords = blockData.ApplyBlockOffset(block);

                if (BlockIgnoreFlags != null)
                {
                    foreach (var blockIgnoreFlag in BlockIgnoreFlags)
                    {
                        if (file.TestFlag(blockIgnoreFlag.Name, adjustedCoords.x, adjustedCoords.z))
                            goto nextBlock; // D: Goto?!? What a maniac.
                    }
                }

                var isSecondaryTerrain = SecondaryTerrainFlag == null ? false :
                        file.TestFlag(SecondaryTerrainFlag.Name, adjustedCoords.x, adjustedCoords.z);

                //Block should be converted
                var success = ConvertBlock(file, block, isSecondaryTerrain, itemChunk);
                if (success)
                    itemCount++;

                nextBlock:;
            }

            if (ItemCountStatistic != null)
            {
                file.AddStatistic(ItemCountStatistic.Name, itemCount);
            }
        }

        private bool ConvertBlock(GBXFile file, Block block, bool isSecondaryTerrain, Challenge03043040 itemChunk)
        {
            var blockName = block.BlockName.Content;
            var blockData = _blockNameDict[blockName];
            var itemInfo = blockData.GetItemInfo(new Identifier(block, isSecondaryTerrain));

            if (itemInfo == null)
                return false;

            //Placing Item
            itemInfo.PlaceRelToBlock(file, block, itemChunk, Collection, DefaultAuthor.Content);
            return true;
        }

        private class BlockDataLoc
        {
            internal BlockToItem bti;
            internal (byte x, byte y, byte z, byte rot) itemCoords;

            public BlockDataLoc(BlockToItem bti, (byte x, byte y, byte z, byte rot) itemCoords)
            {
                this.bti = bti;
                this.itemCoords = itemCoords;
            }
        }
    }
}
