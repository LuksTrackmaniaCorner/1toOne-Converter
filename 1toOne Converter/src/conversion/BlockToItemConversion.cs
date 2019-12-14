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
                _blockNameDict.Add(block.BlockName, block);
                if(block.SecondaryTerrainName is string s)
                {
                    _blockNameDict.Add(s, block);
                }
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
                if (BlockIgnoreFlags != null)
                {
                    foreach (var blockIgnoreFlag in BlockIgnoreFlags)
                    {
                        if (file.TestFlag(blockIgnoreFlag.Name, block.Coords.X, block.Coords.Z))
                            goto nextBlock; // D: Goto?!? What a maniac.
                    }
                }

                //Block cannot not be ignored
                var success = ConvertBlock(file, block, itemChunk);
                if (success)
                    itemCount++;

                nextBlock:;
            }

            if (ItemCountStatistic != null)
            {
                file.AddStatistic(ItemCountStatistic.Name, itemCount);
            }
        }

        private bool ConvertBlock(GBXFile file, Block block, Challenge03043040 itemChunk)
        {
            bool isSecondaryTerrain;
            if (SecondaryTerrainFlag != null)
            {
                isSecondaryTerrain = file.TestFlag(SecondaryTerrainFlag.Name, block.Coords.X, block.Coords.Z);
            }
            else
            {
                isSecondaryTerrain = false;
            }

            var blockName = block.BlockName.Content;
            if (_blockNameDict.ContainsKey(blockName))
            {
                var blockData = _blockNameDict[blockName];
                var itemInfo = blockData.GetItemInfo(new Identifier(block, isSecondaryTerrain));

                if (itemInfo != null)
                {
                    //Getting item data
                    itemInfo.PlaceRelToBlock(file, block, itemChunk, Collection, DefaultAuthor.Content);
                    return true;
                }
            }

            return false;
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

        public void PropagateClips(GBXFile file, string ReferenceBlockName, string referenceClip)
        {
            Initialize();

            var blockChunk = (Challenge0304301F)file.GetChunk(Chunk.challenge0304301FKey);

            //Map blocktoitem objects for all blocks
            var blockDataMap = new BlockDataLoc[GBXFile.MaxMapXSize, GBXFile.MaxMapYSize, GBXFile.MaxMapZSize];
            foreach (var block in blockChunk.Blocks)
            {
                ItemInfo itemInfo;
                bool isSecondaryTerrain = SecondaryTerrainFlag == null ? false : file.TestFlag(SecondaryTerrainFlag.Name, block.Coords.X, block.Coords.Z);

                foreach (var blockData in Blocks)
                {

                    itemInfo = blockData.GetItemInfo(new Identifier(block, isSecondaryTerrain));
                    if (itemInfo != null)
                    {
                        //Find the place in the BlockData-Tree where the clip should be stored
                        BlockToItem clipLoc = null;
                        foreach (var child in blockData.childrenList)
                        {
                            if (child is BlockVariantData blockVariantData && blockVariantData._variant is byte variant && (block.Flags.Value & 0x3F) == variant)
                            {
                                clipLoc = blockVariantData;
                                break;
                            }
                        }
                        clipLoc ??= blockData;

                        //Find the cell where the item should be placed.
                        var blockRot = block.Rot.Value;
                        var adjustedRot = (byte)((blockRot + itemInfo.RotOffset) % 4);
                        var adjustedBlockCoords = ItemInfo.ApplyBlockOffset(block, itemInfo.BlockSize, itemInfo.Offset);

                        //Place a referenve to the BlockData-Object and the position of the item
                        //At all cells where the block is.
                        (byte x, byte z) blockDimensions;
                        if (block.Rot.Value % 2 == 0)
                            blockDimensions = itemInfo.BlockSize;
                        else
                            blockDimensions = (itemInfo.BlockSize.z, itemInfo.BlockSize.x);

                        for (int x = 0; x < blockDimensions.x; x++)
                        {
                            for (int z = 0; z < blockDimensions.z; z++)
                            {
                                var itemCoords = (adjustedBlockCoords.x, adjustedBlockCoords.y, adjustedBlockCoords.z, adjustedRot);
                                blockDataMap[block.Coords.X + x, block.Coords.Y, block.Coords.Z + z] = new BlockDataLoc(clipLoc, itemCoords);
                            }
                        }
                        break;
                    }
                }
            }

            //Get Clips from the referenceblock
            foreach (var block in blockChunk.Blocks)
                if (block.BlockName.Content == ReferenceBlockName)
                    ConvertBlock(file, block, null);

            //Add clips to other blocks, avoid doubles.
            var clips = file.GetClips(referenceClip);
            foreach (var clip in clips)
            {
                BlockDataLoc blockDataLoc = null;
                for (int i = 0; i < 5 && clip.Y - i >= 0; i++) //TODO add const var for 5. it is the maximum amount the converter looks downwards
                {
                    blockDataLoc = blockDataMap[clip.X, clip.Y - i, clip.Z];
                    if (blockDataLoc != null)
                    {
                        break;
                    }
                }

                if (blockDataLoc != null)
                {
                    //Add clipp to blockDataLoc.
                    (int x, int z) newClipAbsCoords = clip.Rot switch
                    {
                        0 => (clip.X, clip.Z + 1),
                        1 => (clip.X - 1, clip.Z),
                        2 => (clip.X, clip.Z - 1),
                        3 => (clip.X + 1, clip.Z),
                        _ => throw new InternalException()
                    };

                    int dx = blockDataLoc.itemCoords.x - newClipAbsCoords.x;
                    int dz = blockDataLoc.itemCoords.z - newClipAbsCoords.z;

                    (int x, int z) newClipRelCoords = blockDataLoc.itemCoords.rot switch
                    {
                        0 => (-dx, -dz),
                        1 => (-dz, +dx),
                        2 => (+dx, +dz),
                        3 => (+dz, -dx),
                        _ => throw new InternalException()
                    };

                    var newClip = new Clip(
                        clip.Name,
                        (short)newClipRelCoords.x,
                        (short)(clip.Y - blockDataLoc.itemCoords.y),
                        (short)newClipRelCoords.z,
                        (byte)((clip.Rot - blockDataLoc.itemCoords.rot + 6) % 4)
                    );

                    if (blockDataLoc.bti.Clips == null)
                    {
                        blockDataLoc.bti.Clips = new Clip[] { newClip };
                    }
                    else if (!blockDataLoc.bti.Clips.Contains(newClip))
                    {
                        blockDataLoc.bti.Clips = blockDataLoc.bti.Clips.Concat(newClip.AsEnumerable()).ToArray();
                    }
                }
            }
        }

        public void AddPylonsToClips(string clipName, short heightOffset)
        {
            Initialize();

            foreach(var blockData in Blocks)
            {
                var enumerator = blockData.GetTreeEnumerator();
                while (enumerator.MoveNext())
                {
                    var leaf = enumerator.Current.Peek();
                    //if (leaf is BlockTypeData btd && btd.TypeOfBlock == Type.Air)
                    if(leaf is BlockVariantData)
                    {
                        //Add Pylons
                        foreach(var blockToItem in enumerator.Current)
                        {
                            foreach(var clip in blockToItem.Clips ?? Enumerable.Empty<Clip>())
                            {
                                if(clip.Name == clipName)
                                {
                                    leaf.MultiPylons ??= new MultiPylon[0];

                                    var pylon = new MultiPylon
                                    {
                                        Pos = PylonPosition.Both,
                                        Type = PylonType.Top,
                                        X = clip.X,
                                        Y = (short)(clip.Y + heightOffset),
                                        Z = clip.Z,
                                        Rot = MultiPylon.GetRot(clip.Rot)
                                    };
                                    if(!leaf.MultiPylons.Contains(pylon))
                                    {
                                        leaf.MultiPylons = leaf.MultiPylons.Concat(pylon.AsEnumerable()).ToArray();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddPylonsToGround(string flagname)
        {
            Initialize();

            foreach(var blockData in Blocks)
            {
                if (blockData.BlockXSize == 1 && blockData.BlockZSize == 1)
                {
                    var enumerator = blockData.GetTreeEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var leaf = enumerator.Current.Peek();
                        if (leaf.Flags is Flag[] flags && flags.Length == 1 && flags.Single().Name == flagname)
                        {
                            leaf.MultiPylons ??= new MultiPylon[0];

                            var pylon = new MultiPylon
                            {
                                Pos = PylonPosition.Both,
                                Type = PylonType.Bottom,
                                X = 0,
                                Y = 0,
                                Z = 0,
                                Rot = MultiRot.All
                            };
                            if (!leaf.MultiPylons.Contains(pylon))
                            {
                                leaf.MultiPylons = leaf.MultiPylons.Concat(pylon.AsEnumerable()).ToArray();
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine(blockData.BlockName);
                }
            }
        }
    }
}
